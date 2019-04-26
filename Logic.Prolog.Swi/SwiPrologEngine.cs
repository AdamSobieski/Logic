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
    public sealed class SwiPrologEngine : IDisposable, IScriptableObject
    {
        public SwiPrologEngine()
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

            modules = new List<SwiPrologModule>();
        }

        private List<SwiPrologModule> modules;

        [ScriptMember("createModule")]
        public SwiPrologModule CreateModule()
        {
            string name;
            var random = new Random((int)DateTime.Now.Ticks);
            do
            {
                name = "m_" + random.Next().ToString();
            }
            while (modules.Find(m => m.Name == name) != null);

            var newModule = new SwiPrologModule(this, name);
            modules.Add(newModule);
            return newModule;
        }

        [ScriptMember("createModule")]
        public SwiPrologModule CreateModule(string name)
        {
            if (modules.Find(m => m.Name == name) != null) throw new ArgumentException("A module with that name already exists.", nameof(name));

            var newModule = new SwiPrologModule(this, name);
            modules.Add(newModule);
            return newModule;
        }

        [ScriptMember("createModule")]
        public SwiPrologModule CreateModule(string name, dynamic settings)
        {
            if (modules.Find(m => m.Name == name) != null) throw new ArgumentException("A module with that name already exists.", nameof(name));

            var newModule = new SwiPrologModule(this, name, settings);
            modules.Add(newModule);
            return newModule;
        }

        [ScriptMember("atom")]
        public SwiPrologTerm Atom(string name)
        {
            return new SwiPrologTerm(name);
        }

        [ScriptMember("atom")]
        public SwiPrologTerm Atom(int value)
        {
            return new SwiPrologTerm(value);
        }

        [ScriptMember("atom")]
        public SwiPrologTerm Atom(double value)
        {
            return new SwiPrologTerm(value);
        }

        [ScriptMember("variable")]
        public SwiPrologTerm Variable()
        {
            return SwiPrologTerm.Variable();
        }

        [ScriptMember("compound")]
        public SwiPrologTerm Compound(string functor, params SwiPrologTerm[] args)
        {
            return SwiPrologTerm.Compound(functor, new SwiPrologTermVector(args));
        }

        [ScriptMember("list")]
        public SwiPrologTerm List(SwiPrologTerm initial)
        {
            return SwiPrologTerm.Tail(initial);
        }

        [ScriptMember("string")]
        public SwiPrologTerm String(string text)
        {
            return SwiPrologTerm.String(text);
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

    public sealed class SwiPrologModule : IDisposable
    {
        internal SwiPrologModule(SwiPrologEngine prolog, string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            module = name;
            foreignPredicates = new List<Delegate>();
        }
        internal SwiPrologModule(SwiPrologEngine prolog, string name, dynamic settings)
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
            return SwiPrologQuery.Call(module, "assert(" + term + ")");
        }

        [ScriptMember("retract")]
        public bool Retract(string term)
        {
            return SwiPrologQuery.Call(module, "retract(" + term + ")");
        }

        [ScriptMember("contains")]
        public bool Contains(string term)
        {
            return SwiPrologQuery.Call(module, term);
        }

        [ScriptMember("assertRule")]
        public bool AssertRule(string head, string body)
        {
            return SwiPrologQuery.Call(module, "assert((" + head + " :- " + body + "))");
        }

        [ScriptMember("retractRule")]
        public bool RetractRule(string head, string body)
        {
            return SwiPrologQuery.Call(module, "retract((" + head + " :- " + body + "))");
        }

        [ScriptMember("call")]
        public bool Call(string goal)
        {
            return SwiPrologQuery.Call(module, goal);
        }

        [ScriptMember("call")]
        public bool Call(SwiPrologTerm goal)
        {
            return Call(goal.ToString());
        }

        [ScriptMember("query")]
        public IEnumerable<SwiPrologQueryResult> Query(string query)
        {
            using (var q = new SwiPrologQuery(module, query))
            {
                foreach (SwiPrologQueryResult v in q.SolutionVariables)
                {
                    yield return v;
                }
            }
        }

        [ScriptMember("query")]
        public IEnumerable<SwiPrologQueryResult> Query(SwiPrologTerm query)
        {
            //return Query(query.ToString());
            var count = query.Arity;
            var array = new SwiPrologTerm[count];
            for (int index = 0; index < count; ++index)
            {
                array[index] = query[index + 1];
            }
            using (var q = new SwiPrologQuery(module, query.Name, new SwiPrologTermVector(array)))
            {
                foreach (SwiPrologQueryResult v in q.SolutionVariables)
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
                    d = new SwiPrologCallback0(() => functor());
                    break;
                case 1:
                    d = new SwiPrologCallback1((SwiPrologTerm term1) => functor(term1));
                    break;
                case 2:
                    d = new SwiPrologCallback2((SwiPrologTerm term1, SwiPrologTerm term2) => functor(term1, term2));
                    break;
                case 3:
                    d = new SwiPrologCallback3((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3) => functor(term1, term2, term3));
                    break;
                case 4:
                    d = new SwiPrologCallback4((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4) => functor(term1, term2, term3, term4));
                    break;
                case 5:
                    d = new SwiPrologCallback5((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5) => functor(term1, term2, term3, term4, term5));
                    break;
                case 6:
                    d = new SwiPrologCallback6((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6) => functor(term1, term2, term3, term4, term5, term6));
                    break;
                case 7:
                    d = new SwiPrologCallback7((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7) => functor(term1, term2, term3, term4, term5, term6, term7));
                    break;
                case 8:
                    d = new SwiPrologCallback8((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, SwiPrologTerm term8) => functor(term1, term2, term3, term4, term5, term6, term7, term8));
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
                    d = new SwiNativeForeignNondeterministicPredicateCallback1((SwiPrologTerm term1, IntPtr control_t) =>
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
                    d = new SwiNativeForeignNondeterministicPredicateCallback2((SwiPrologTerm term1, SwiPrologTerm term2, IntPtr control_t) =>
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
                    d = new SwiNativeForeignNondeterministicPredicateCallback3((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, IntPtr control_t) =>
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
                    d = new SwiNativeForeignNondeterministicPredicateCallback4((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, IntPtr control_t) =>
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
                    d = new SwiNativeForeignNondeterministicPredicateCallback5((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, IntPtr control_t) =>
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
                    d = new SwiNativeForeignNondeterministicPredicateCallback6((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, IntPtr control_t) =>
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
                    d = new SwiNativeForeignNondeterministicPredicateCallback7((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, IntPtr control_t) =>
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
                    d = new SwiNativeForeignNondeterministicPredicateCallback8((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, SwiPrologTerm term8, IntPtr control_t) =>
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
                    if (!(functor is SwiPrologCallback0)) throw new ArgumentException(nameof(functor));
                    break;
                case 1:
                    if (!(functor is SwiPrologCallback1)) throw new ArgumentException(nameof(functor));
                    break;
                case 2:
                    if (!(functor is SwiPrologCallback2)) throw new ArgumentException(nameof(functor));
                    break;
                case 3:
                    if (!(functor is SwiPrologCallback3)) throw new ArgumentException(nameof(functor));
                    break;
                case 4:
                    if (!(functor is SwiPrologCallback4)) throw new ArgumentException(nameof(functor));
                    break;
                case 5:
                    if (!(functor is SwiPrologCallback5)) throw new ArgumentException(nameof(functor));
                    break;
                case 6:
                    if (!(functor is SwiPrologCallback6)) throw new ArgumentException(nameof(functor));
                    break;
                case 7:
                    if (!(functor is SwiPrologCallback7)) throw new ArgumentException(nameof(functor));
                    break;
                case 8:
                    if (!(functor is SwiPrologCallback8)) throw new ArgumentException(nameof(functor));
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
                    if (!(functor is SwiPrologNondeterministicCallback0)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback0((IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((SwiPrologNondeterministicCallback0)functor)(context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((SwiPrologNondeterministicCallback0)functor)(context) ? SWI.Retry(context) : IntPtr.Zero;
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
                    if (!(functor is SwiPrologNondeterministicCallback1)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback1((SwiPrologTerm term1, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((SwiPrologNondeterministicCallback1)functor)(term1, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((SwiPrologNondeterministicCallback1)functor)(term1, context) ? SWI.Retry(context) : IntPtr.Zero;
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
                    if (!(functor is SwiPrologNondeterministicCallback2)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback2((SwiPrologTerm term1, SwiPrologTerm term2, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((SwiPrologNondeterministicCallback2)functor)(term1, term2, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((SwiPrologNondeterministicCallback2)functor)(term1, term2, context) ? SWI.Retry(context) : IntPtr.Zero;
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
                    if (!(functor is SwiPrologNondeterministicCallback3)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback3((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((SwiPrologNondeterministicCallback3)functor)(term1, term2, term3, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((SwiPrologNondeterministicCallback3)functor)(term1, term2, term3, context) ? SWI.Retry(context) : IntPtr.Zero;
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
                    if (!(functor is SwiPrologNondeterministicCallback4)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback4((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((SwiPrologNondeterministicCallback4)functor)(term1, term2, term3, term4, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((SwiPrologNondeterministicCallback4)functor)(term1, term2, term3, term4, context) ? SWI.Retry(context) : IntPtr.Zero;
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
                    if (!(functor is SwiPrologNondeterministicCallback5)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback5((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((SwiPrologNondeterministicCallback5)functor)(term1, term2, term3, term4, term5, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((SwiPrologNondeterministicCallback5)functor)(term1, term2, term3, term4, term5, context) ? SWI.Retry(context) : IntPtr.Zero;
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
                    if (!(functor is SwiPrologNondeterministicCallback6)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback6((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((SwiPrologNondeterministicCallback6)functor)(term1, term2, term3, term4, term5, term6, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((SwiPrologNondeterministicCallback6)functor)(term1, term2, term3, term4, term5, term6, context) ? SWI.Retry(context) : IntPtr.Zero;
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
                    if (!(functor is SwiPrologNondeterministicCallback7)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback7((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((SwiPrologNondeterministicCallback7)functor)(term1, term2, term3, term4, term5, term6, term7, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((SwiPrologNondeterministicCallback7)functor)(term1, term2, term3, term4, term5, term6, term7, context) ? SWI.Retry(context) : IntPtr.Zero;
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
                    if (!(functor is SwiPrologNondeterministicCallback8)) throw new ArgumentException(nameof(functor));
                    d = new SwiNativeForeignNondeterministicPredicateCallback8((SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, SwiPrologTerm term8, IntPtr control_t) =>
                    {
                        dynamic context;

                        switch (SWI.GetNondeterministicCallType(control_t))
                        {
                            case NondeterministicCalltype.FirstCall:
                                context = new ExpandoObject();
                                return ((SwiPrologNondeterministicCallback8)functor)(term1, term2, term3, term4, term5, term6, term7, term8, context) ? SWI.Retry(context) : IntPtr.Zero;
                            case NondeterministicCalltype.Redo:
                                context = SWI.GetContext(control_t);
                                return ((SwiPrologNondeterministicCallback8)functor)(term1, term2, term3, term4, term5, term6, term7, term8, context) ? SWI.Retry(context) : IntPtr.Zero;
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
        public IEnumerable<SwiPrologTerm> GetAssertions()
        {
            foreach (var r in Query("predicate_property(S, dynamic), clause(S, true)"))
            {
                yield return r["S"];
            }
        }

        [ScriptMember("getRules")]
        public IEnumerable<SwiPrologTerm> GetRules()
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