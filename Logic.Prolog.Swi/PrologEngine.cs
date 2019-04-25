/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Swi.Callback;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace Logic.Prolog.Swi
{
    public sealed class PrologEngine : IDisposable, IScriptableObject
    {
        public PrologEngine()
        {
            if (!SWI.IsInitialized)
            {
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (!isWindows)
                    throw new Exception("This project presently is built for 64-bit Windows.");

                bool is64bit = Environment.Is64BitOperatingSystem && Environment.Is64BitProcess;
                if (!is64bit)
                    throw new Exception("This project presently is built for 64-bit Windows.");

                if (!System.IO.Directory.Exists(@"C:\Program Files\swipl"))
                    throw new Exception("This project requites SWI-Prolog (64-bit Windows version) to be installed.");

                Environment.SetEnvironmentVariable("SWI_HOME_DIR", @"C:\Program Files\swipl");
                Environment.SetEnvironmentVariable("Path", @"C:\Program Files\swipl\bin;%Path%");

                string[] parameters = { "-q", "-O", "--nosignals", "--nodebug" };
                SWI.Initialize(parameters);
            }

            modules = new List<PrologModule>();
        }

        private List<PrologModule> modules;

        [ScriptMember("createModule")]
        public PrologModule CreateModule()
        {
            string name;
            var random = new Random((int)DateTime.Now.Ticks);
            do
            {
                name = "m_" + random.Next().ToString();
            }
            while (modules.Find(m => m.Name == name) != null);

            var newModule = new PrologModule(this, name);
            modules.Add(newModule);
            return newModule;
        }

        [ScriptMember("createModule")]
        public PrologModule CreateModule(string name)
        {
            if (modules.Find(m => m.Name == name) != null) throw new ArgumentException("A module with that name already exists.", nameof(name));

            var newModule = new PrologModule(this, name);
            modules.Add(newModule);
            return newModule;
        }

        [ScriptMember("createModule")]
        public PrologModule CreateModule(string name, dynamic settings)
        {
            if (modules.Find(m => m.Name == name) != null) throw new ArgumentException("A module with that name already exists.", nameof(name));

            var newModule = new PrologModule(this, name, settings);
            modules.Add(newModule);
            return newModule;
        }

        [ScriptMember("atom")]
        public PrologTerm Atom(string name)
        {
            return new PrologTerm(name);
        }

        [ScriptMember("atom")]
        public PrologTerm Atom(int value)
        {
            return new PrologTerm(value);
        }

        [ScriptMember("atom")]
        public PrologTerm Atom(double value)
        {
            return new PrologTerm(value);
        }

        [ScriptMember("variable")]
        public PrologTerm Variable()
        {
            return PrologTerm.Variable();
        }

        [ScriptMember("compound")]
        public PrologTerm Compound(string functor, params PrologTerm[] args)
        {
            return PrologTerm.Compound(functor, new PrologTermVector(args));
        }

        [ScriptMember("list")]
        public PrologTerm List(PrologTerm initial)
        {
            return PrologTerm.Tail(initial);
        }

        [ScriptMember("string")]
        public PrologTerm String(string text)
        {
            return PrologTerm.String(text);
        }

        [NoScriptAccess]
        void IDisposable.Dispose()
        {
            SWI.Cleanup();
        }

        [NoScriptAccess]
        void IScriptableObject.OnExposedToScriptCode(ScriptEngine engine)
        {

        }
    }

    public sealed class PrologModule : IDisposable
    {
        internal PrologModule(PrologEngine prolog, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            module = name;
            foreignPredicates = new List<Delegate>();
        }
        internal PrologModule(PrologEngine prolog, string name, dynamic settings)
            : this(prolog, name) { }

        private string module;
        private List<Delegate> foreignPredicates;

        [ScriptMember("name")]
        public string Name
        {
            get
            {
                return module;
            }
        }

        [ScriptMember("assert")]
        public bool Assert(string term)
        {
            return PrologQuery.Call(module, "assert(" + term + ")");
        }

        [ScriptMember("retract")]
        public bool Retract(string term)
        {
            return PrologQuery.Call(module, "retract(" + term + ")");
        }

        [ScriptMember("contains")]
        public bool Contains(string term)
        {
            return PrologQuery.Call(module, term);
        }

        [ScriptMember("assertRule")]
        public bool AssertRule(string head, string body)
        {
            return PrologQuery.Call(module, "assert((" + head + " :- " + body + "))");
        }

        [ScriptMember("retractRule")]
        public bool RetractRule(string head, string body)
        {
            return PrologQuery.Call(module, "retract((" + head + " :- " + body + "))");
        }

        [ScriptMember("call")]
        public bool Call(string goal)
        {
            return PrologQuery.Call(module, goal);
        }

        [ScriptMember("call")]
        public bool Call(PrologTerm goal)
        {
            return Call(goal.ToString());
        }

        [ScriptMember("query")]
        public IEnumerable<PrologQueryResult> Query(string query)
        {
            using (var q = new PrologQuery(module, query))
            {
                foreach (PrologQueryResult v in q.SolutionVariables)
                {
                    yield return v;
                }
            }
        }

        [ScriptMember("query")]
        public IEnumerable<PrologQueryResult> Query(PrologTerm query)
        {
            //return Query(query.ToString());
            var count = query.Arity;
            var array = new PrologTerm[count];
            for (int index = 0; index < count; ++index)
            {
                array[index] = query[index + 1];
            }
            using (var q = new PrologQuery(module, query.Name, new PrologTermVector(array)))
            {
                foreach (PrologQueryResult v in q.SolutionVariables)
                {
                    yield return v;
                }
            }
        }

        [ScriptMember("addPredicate")]
        public bool AddPredicate(string name, int arity, dynamic functor)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            // can auto-detect arity using predicate.toString and processing for 'function(*, *, *)'
            if (arity < 0) throw new ArgumentOutOfRangeException(nameof(arity));
            if (functor == null) throw new ArgumentNullException(nameof(functor));
            if (functor is Delegate) return AddPredicate(name, arity, (Delegate)functor);

            Delegate d;

            switch (arity)
            {
                case 0:
                    d = new PrologCallback0(() => functor());
                    break;
                case 1:
                    d = new PrologCallback1((PrologTerm term1) => functor(term1));
                    break;
                case 2:
                    d = new PrologCallback2((PrologTerm term1, PrologTerm term2) => functor(term1, term2));
                    break;
                case 3:
                    d = new PrologCallback3((PrologTerm term1, PrologTerm term2, PrologTerm term3) => functor(term1, term2, term3));
                    break;
                case 4:
                    d = new PrologCallback4((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4) => functor(term1, term2, term3, term4));
                    break;
                case 5:
                    d = new PrologCallback5((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5) => functor(term1, term2, term3, term4, term5));
                    break;
                case 6:
                    d = new PrologCallback6((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6) => functor(term1, term2, term3, term4, term5, term6));
                    break;
                case 7:
                    d = new PrologCallback7((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7) => functor(term1, term2, term3, term4, term5, term6, term7));
                    break;
                case 8:
                    d = new PrologCallback8((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, PrologTerm term8) => functor(term1, term2, term3, term4, term5, term6, term7, term8));
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (SWI.RegisterForeign(module, name, arity, d, ForeignSwitches.None))
            {
                foreignPredicates.Add(d);
                return true;
            }
            else return false;
        }

        [ScriptMember("addPredicateNondeterministic")]
        public bool AddPredicateNondeterministic(string name, int arity, dynamic functor)
        {
            // http://www.swi-prolog.org/pldoc/man?section=foreignnondet

            if (name == null) throw new ArgumentNullException(nameof(name));
            // can auto-detect arity using predicate.toString and processing for 'function(*, *, *)'
            if (arity < 0) throw new ArgumentOutOfRangeException(nameof(arity));
            if (functor == null) throw new ArgumentNullException(nameof(functor));
            if (functor is Delegate) return AddPredicateNondeterministic(name, arity, (Delegate)functor);

            // TO DO: should we add the context to a list to prevent garbage collection and then remove it from a list upon completion

            Delegate d;

            switch (arity)
            {
                case 0:
                    d = new SwiNativeForeignNondeterministicPredicateCallback0((IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return functor(context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return functor(context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 1:
                    d = new SwiNativeForeignNondeterministicPredicateCallback1((PrologTerm term1, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return functor(term1, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return functor(term1, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 2:
                    d = new SwiNativeForeignNondeterministicPredicateCallback2((PrologTerm term1, PrologTerm term2, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return functor(term1, term2, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return functor(term1, term2, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 3:
                    d = new SwiNativeForeignNondeterministicPredicateCallback3((PrologTerm term1, PrologTerm term2, PrologTerm term3, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return functor(term1, term2, term3, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return functor(term1, term2, term3, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 4:
                    d = new SwiNativeForeignNondeterministicPredicateCallback4((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return functor(term1, term2, term3, term4, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return functor(term1, term2, term3, term4, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 5:
                    d = new SwiNativeForeignNondeterministicPredicateCallback5((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return functor(term1, term2, term3, term4, term5, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return functor(term1, term2, term3, term4, term5, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 6:
                    d = new SwiNativeForeignNondeterministicPredicateCallback6((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return functor(term1, term2, term3, term4, term5, term6, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return functor(term1, term2, term3, term4, term5, term6, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 7:
                    d = new SwiNativeForeignNondeterministicPredicateCallback7((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return functor(term1, term2, term3, term4, term5, term6, term7, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return functor(term1, term2, term3, term4, term5, term6, term7, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 8:
                    d = new SwiNativeForeignNondeterministicPredicateCallback8((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, PrologTerm term8, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return functor(term1, term2, term3, term4, term5, term6, term7, term8, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return functor(term1, term2, term3, term4, term5, term6, term7, term8, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (SWI.RegisterForeign(module, name, arity, d, ForeignSwitches.Nondeterministic))
            {
                foreignPredicates.Add(d);
                return true;
            }
            else return false;
        }

        [NoScriptAccess]
        public bool AddPredicate(string name, int arity, Delegate functor)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (arity < 0) throw new ArgumentOutOfRangeException(nameof(arity));
            if (functor == null) throw new ArgumentNullException(nameof(functor));

            switch (arity)
            {
                case 0:
                    if (!(functor is PrologCallback0)) throw new ArgumentException(nameof(functor));
                    break;
                case 1:
                    if (!(functor is PrologCallback1)) throw new ArgumentException(nameof(functor));
                    break;
                case 2:
                    if (!(functor is PrologCallback2)) throw new ArgumentException(nameof(functor));
                    break;
                case 3:
                    if (!(functor is PrologCallback3)) throw new ArgumentException(nameof(functor));
                    break;
                case 4:
                    if (!(functor is PrologCallback4)) throw new ArgumentException(nameof(functor));
                    break;
                case 5:
                    if (!(functor is PrologCallback5)) throw new ArgumentException(nameof(functor));
                    break;
                case 6:
                    if (!(functor is PrologCallback6)) throw new ArgumentException(nameof(functor));
                    break;
                case 7:
                    if (!(functor is PrologCallback7)) throw new ArgumentException(nameof(functor));
                    break;
                case 8:
                    if (!(functor is PrologCallback8)) throw new ArgumentException(nameof(functor));
                    break;
                default:
                    throw new NotImplementedException();
            }
            if (SWI.RegisterForeign(module, name, arity, functor, ForeignSwitches.None))
            {
                foreignPredicates.Add(functor);
                return true;
            }
            else return false;
        }

        [NoScriptAccess]
        public bool AddPredicateNondeterministic(string name, int arity, Delegate functor)
        {
            // http://www.swi-prolog.org/pldoc/man?section=foreignnondet

            if (name == null) throw new ArgumentNullException(nameof(name));
            if (arity < 0) throw new ArgumentOutOfRangeException(nameof(arity));
            if (functor == null) throw new ArgumentNullException(nameof(functor));

            // TO DO: is the pointer to context the same from the back-and-forth marshalling; should we add it to a list to prevent garbage collection and then remove it from a list upon completion

            Delegate d;

            switch (arity)
            {
                case 0:
                    if (!(functor is PrologNondeterministicCallback0)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback0((IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((PrologNondeterministicCallback0)functor)(context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((PrologNondeterministicCallback0)functor)(context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 1:
                    if (!(functor is PrologNondeterministicCallback1)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback1((PrologTerm term1, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((PrologNondeterministicCallback1)functor)(term1, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((PrologNondeterministicCallback1)functor)(term1, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 2:
                    if (!(functor is PrologNondeterministicCallback2)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback2((PrologTerm term1, PrologTerm term2, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((PrologNondeterministicCallback2)functor)(term1, term2, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((PrologNondeterministicCallback2)functor)(term1, term2, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 3:
                    if (!(functor is PrologNondeterministicCallback3)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback3((PrologTerm term1, PrologTerm term2, PrologTerm term3, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((PrologNondeterministicCallback3)functor)(term1, term2, term3, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((PrologNondeterministicCallback3)functor)(term1, term2, term3, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 4:
                    if (!(functor is PrologNondeterministicCallback4)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback4((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((PrologNondeterministicCallback4)functor)(term1, term2, term3, term4, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((PrologNondeterministicCallback4)functor)(term1, term2, term3, term4, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 5:
                    if (!(functor is PrologNondeterministicCallback5)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback5((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((PrologNondeterministicCallback5)functor)(term1, term2, term3, term4, term5, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((PrologNondeterministicCallback5)functor)(term1, term2, term3, term4, term5, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 6:
                    if (!(functor is PrologNondeterministicCallback6)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback6((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((PrologNondeterministicCallback6)functor)(term1, term2, term3, term4, term5, term6, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((PrologNondeterministicCallback6)functor)(term1, term2, term3, term4, term5, term6, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 7:
                    if (!(functor is PrologNondeterministicCallback7)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback7((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((PrologNondeterministicCallback7)functor)(term1, term2, term3, term4, term5, term6, term7, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((PrologNondeterministicCallback7)functor)(term1, term2, term3, term4, term5, term6, term7, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                case 8:
                    if (!(functor is PrologNondeterministicCallback8)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback8((PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, PrologTerm term8, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((PrologNondeterministicCallback8)functor)(term1, term2, term3, term4, term5, term6, term7, term8, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((PrologNondeterministicCallback8)functor)(term1, term2, term3, term4, term5, term6, term7, term8, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Pruned:
                                context = SWI.GetContext(control_t);
                                context = null;
                                return new IntPtr(1);
                            default:
                                throw new InvalidProgramException();
                        }
                    });
                    break;
                default:
                    throw new NotImplementedException();
            }

            if (SWI.RegisterForeign(module, name, arity, d, ForeignSwitches.Nondeterministic))
            {
                foreignPredicates.Add(d);
                return true;
            }
            else return false;
        }

        [ScriptMember("getAssertions")]
        public IEnumerable<PrologTerm> GetAssertions()
        {
            foreach (var r in Query("predicate_property(S, dynamic), clause(S, true)"))
            {
                yield return r["S"];
            }
        }

        [ScriptMember("getRules")]
        public IEnumerable<PrologTerm> GetRules()
        {
            foreach (var r in Query(@"predicate_property(S, dynamic), clause(S, T), T \= true, X=(S :- T)"))
            {
                yield return r["X"];
            }
        }

        [NoScriptAccess]
        void IDisposable.Dispose()
        {

        }
    }
}