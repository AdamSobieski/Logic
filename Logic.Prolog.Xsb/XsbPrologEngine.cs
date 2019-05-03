/*********************************************************
* 
*  Authors:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Xsb.Callbacks;
using Logic.Prolog.Xsb.Exceptions;
using Logic.Prolog.Xsb.Initialization;
using Microsoft.ClearScript;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;

namespace Logic.Prolog.Xsb
{
    public class XsbPrologEngine : IDisposable, IScriptableObject
    {
        public XsbPrologEngine()
            : this(new XsbPrologInitializationSettings()) { }
        public XsbPrologEngine(XsbPrologInitializationSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!isWindows)
                throw new Exception("This project presently is built for 64-bit Windows.");

            bool is64bit = Environment.Is64BitOperatingSystem && Environment.Is64BitProcess;
            if (!is64bit)
                throw new Exception("This project presently is built for 64-bit Windows.");

            if (!Directory.Exists(settings.HomeDirectory))
                throw new Exception("The specified home directory for XSB Prolog does not exist.");

            if (settings.SetHomeDirectoryEnvironmentVariable)
                Environment.SetEnvironmentVariable("XSB_DIR", settings.HomeDirectory);

            if (settings.PrependBinaryDirectoryToPath)
                Environment.SetEnvironmentVariable("Path", settings.BinaryDirectory + Path.PathSeparator + "%Path%");

            string[] parameters = settings.GenerateParameters();

            if (xsb.xsb_init(parameters.Length, parameters) != 0)
            {
                throw new XsbException("Failed to initialize XSB engine.\n" + "Error code: " + xsb.xsb_get_init_error_type() + "\n" + "Error message: " + xsb.xsb_get_init_error_message());
            }

            foreignPredicates = new List<Delegate>();
        }

        private List<Delegate> foreignPredicates;

        [ScriptMember("variable")]
        public XsbPrologTerm Variable()
        {
            return new XsbPrologTerm(xsb.p2p_new());
        }
        [ScriptMember("atom")]
        public XsbPrologTerm Atom(string name)
        {
            var retval = new XsbPrologTerm(xsb.p2p_new());
            retval.BindAtom(name);
            return retval;
        }
        [ScriptMember("integer")]
        public XsbPrologTerm Integer(int value)
        {
            var retval = new XsbPrologTerm(xsb.p2p_new());
            retval.BindInteger(value);
            return retval;
        }
        [ScriptMember("float")]
        public XsbPrologTerm Float(double value)
        {
            var retval = new XsbPrologTerm(xsb.p2p_new());
            retval.BindDouble(value);
            return retval;
        }
        [ScriptMember("compound")]
        public XsbPrologTerm Compound(string module, string functor, params XsbPrologTerm[] args)
        {
            Contract.Requires(module != null);
            Contract.Requires(functor != null);
            Contract.Requires(args != null);

            int arity = args.Length;
            var term = xsb.p2p_new();
            xsb.c2p_functor_in_mod(module, functor, arity, term);
            for (int i = 0; i < arity; ++i)
            {
                xsb.p2p_unify(xsb.p2p_arg(term, i + 1), args[i].term);
            }

            return new XsbPrologTerm(term);
        }
        [ScriptMember("compound")]
        public XsbPrologTerm Compound(string functor, params XsbPrologTerm[] args)
        {
            Contract.Requires(functor != null);
            Contract.Requires(args != null);

            int arity = args.Length;
            var term = xsb.p2p_new();
            xsb.c2p_functor(functor, arity, term);
            for (int i = 0; i < arity; ++i)
            {
                xsb.p2p_unify(xsb.p2p_arg(term, i + 1), args[i].term);
            }

            return new XsbPrologTerm(term);
        }
        [ScriptMember("list")]
        public XsbPrologTerm List(XsbPrologTerm head, XsbPrologTerm tail)
        {
            var term = new XsbPrologTerm(xsb.p2p_new());
            term.BindList();
            term.Head.Unify(head);
            term.Tail.Unify(tail);
            return term;
        }
        [ScriptMember("nil")]
        public XsbPrologTerm Nil()
        {
            var term = new XsbPrologTerm(xsb.p2p_new());
            term.BindNil();
            return term;
        }
        [ScriptMember("string")]
        public XsbPrologTerm String(string text)
        {
            throw new NotImplementedException();
        }



        [ScriptMember("call")]
        public bool Call(XsbPrologTerm goal)
        {
            ulong reg1 = xsb.reg_term(1);
            xsb.p2p_unify(goal.term, reg1);
            return xsb.xsb_command() == 0;
        }
        [ScriptMember("call")]
        public bool Call(string goal)
        {
            if (!goal.EndsWith("."))
            {
                goal += ".";
            }
            return xsb.xsb_command_string(goal) == 0;
        }
        [ScriptMember("query")]
        public IEnumerable<XsbPrologQueryResult> Query(XsbPrologTerm query)
        {
            ulong reg1 = xsb.reg_term(1);
            xsb.p2p_unify(query.term, reg1);
            return new XsbPrologQuery(xsb.xsb_query() == 0);
        }
        [ScriptMember("query")]
        public IEnumerable<XsbPrologQueryResult> Query(string query)
        {
            if (!query.EndsWith("."))
            {
                query += ".";
            }
            return new XsbPrologQuery(xsb.xsb_query_string(query) == 0);
        }



        [ScriptMember("assert")]
        public bool Assert(string term)
        {
            return xsb.xsb_command_string("assert(" + term + ").") == 0;
        }
        [ScriptMember("retract")]
        public bool Retract(string term)
        {
            return xsb.xsb_command_string("retract(" + term + ").") == 0;
        }
        [ScriptMember("contains")]
        public bool Contains(string term)
        {
            return Call(term);
        }



        [ScriptMember("assertRule")]
        public bool AssertRule(string head, string body)
        {
            return xsb.xsb_command_string("assert((" + head + " :- " + body + ")).") == 0;
        }
        [ScriptMember("retractRule")]
        public bool RetractRule(string head, string body)
        {
            return xsb.xsb_command_string("retract((" + head + " :- " + body + ")).") == 0;
        }



        [ScriptMember("addPredicate")]
        public bool AddPredicate(string name, int arity, dynamic functor)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (arity < 0) throw new ArgumentOutOfRangeException(nameof(arity));
            if (functor == null) throw new ArgumentNullException(nameof(functor));

            throw new NotImplementedException();
        }
        [NoScriptAccess]
        public bool AddPredicate(string name, int arity, Delegate functor)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (arity < 0) throw new ArgumentOutOfRangeException(nameof(arity));
            if (functor == null) throw new ArgumentNullException(nameof(functor));

            XsbForeignPredicateCallback func;

            switch (arity)
            {
                case 0:
                    func = new XsbForeignPredicateCallback(() =>
                    {
                        //xsb.xsb_query_save(0);
                        bool retval = ((XsbPrologCallback0)functor)();
                        //xsb.xsb_query_restore();
                        return retval ? 1 : 0;
                    });
                    break;
                case 1:
                    func = new XsbForeignPredicateCallback(() =>
                    {
                        XsbPrologTerm arg1 = new XsbPrologTerm(xsb.reg_term(1));
                        //xsb.xsb_query_save(1);
                        bool retval = ((XsbPrologCallback1)functor)(arg1);
                        //xsb.xsb_query_restore();
                        return retval ? 1 : 0;
                    });
                    break;
                case 2:
                    func = new XsbForeignPredicateCallback(() =>
                    {
                        XsbPrologTerm arg1 = new XsbPrologTerm(xsb.reg_term(1));
                        XsbPrologTerm arg2 = new XsbPrologTerm(xsb.reg_term(2));
                        //xsb.xsb_query_save(2);
                        bool retval = ((XsbPrologCallback2)functor)(arg1, arg2);
                        //xsb.xsb_query_restore();
                        return retval ? 1 : 0;
                    });
                    break;
                case 3:
                    func = new XsbForeignPredicateCallback(() =>
                    {
                        XsbPrologTerm arg1 = new XsbPrologTerm(xsb.reg_term(1));
                        XsbPrologTerm arg2 = new XsbPrologTerm(xsb.reg_term(2));
                        XsbPrologTerm arg3 = new XsbPrologTerm(xsb.reg_term(3));
                        //xsb.xsb_query_save(3);
                        bool retval = ((XsbPrologCallback3)functor)(arg1, arg2, arg3);
                        //xsb.xsb_query_restore();
                        return retval ? 1 : 0;
                    });
                    break;
                case 4:
                    func = new XsbForeignPredicateCallback(() =>
                    {
                        XsbPrologTerm arg1 = new XsbPrologTerm(xsb.reg_term(1));
                        XsbPrologTerm arg2 = new XsbPrologTerm(xsb.reg_term(2));
                        XsbPrologTerm arg3 = new XsbPrologTerm(xsb.reg_term(3));
                        XsbPrologTerm arg4 = new XsbPrologTerm(xsb.reg_term(4));
                        //xsb.xsb_query_save(4);
                        bool retval = ((XsbPrologCallback4)functor)(arg1, arg2, arg3, arg4);
                        //xsb.xsb_query_restore();
                        return retval ? 1 : 0;
                    });
                    break;
                default:
                    throw new NotImplementedException();
            }

            if(xsb.xsb_add_c_predicate(null, name, arity, func) == 0)
            {
                foreignPredicates.Add(func);
                return true;
            }
            return false;
        }



        [ScriptMember("getAssertions")]
        public IEnumerable<XsbPrologTerm> GetAssertions()
        {
            foreach (var r in Query("predicate_property(S, dynamic), clause(S, true)"))
            {
                yield return r.Answer[1];
            }
        }

        [ScriptMember("getRules")]
        public IEnumerable<XsbPrologTerm> GetRules()
        {
            foreach (var r in Query(@"predicate_property(S, dynamic), clause(S, T), T \= true, X=(S :- T)"))
            {
                yield return r.Answer[3];
            }
        }



        [NoScriptAccess]
        void IDisposable.Dispose()
        {
            xsb.xsb_close();
        }

        [NoScriptAccess]
        void IScriptableObject.OnExposedToScriptCode(ScriptEngine engine)
        {

        }
    }
}