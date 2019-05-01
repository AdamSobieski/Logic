/*********************************************************
* 
*  Authors:        Adam Sobieski
*
*********************************************************/

using Microsoft.ClearScript;
using System;
using System.Diagnostics;

namespace Logic.Prolog.Xsb
{
    public struct XsbPrologTerm
    {
        internal XsbPrologTerm(ulong term)
        {
            this.term = term;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        internal ulong term;

        internal void BindInteger(int value)
        {
            xsb.c2p_int(value, term);
        }

        internal void BindDouble(double value)
        {
            xsb.c2p_float(value, term);
        }

        internal void BindAtom(string symbol)
        {
            xsb.c2p_string(symbol, term);
        }

        internal void BindFunctor(string symbol, int arity)
        {
            xsb.c2p_functor(symbol, arity, term);
        }

        internal void BindFunctorInModule(string module, string symbol, int arity)
        {
            xsb.c2p_functor_in_mod(module, symbol, arity, term);
        }

        internal void BindList()
        {
            xsb.c2p_list(term);
        }

        internal void BindNil()
        {
            xsb.c2p_nil(term);
        }

        [ScriptMember("unify")]
        public bool Unify(XsbPrologTerm term)
        {
            return xsb.p2p_unify(this.term, term.term) != 0;
        }

        [ScriptMember("isVariable")]
        public bool IsVariable
        {
            get
            {
                return xsb.is_var(term) != 0;
            }
        }

        [ScriptMember("isInteger")]
        public bool IsInteger
        {
            get
            {
                return xsb.is_int(term) != 0;
            }
        }

        [ScriptMember("isFloat")]
        public bool IsFloat
        {
            get
            {
                return xsb.is_float(term) != 0;
            }
        }

        [ScriptMember("isString")]
        public bool IsString
        {
            get
            {
                return xsb.is_string(term) != 0;
            }
        }

        [ScriptMember("isAtom")]
        public bool IsAtom
        {
            get
            {
                return xsb.is_atom(term) != 0;
            }
        }

        [ScriptMember("isList")]
        public bool IsList
        {
            get
            {
                return xsb.is_list(term) != 0;
            }
        }

        [ScriptMember("isNil")]
        public bool IsNil
        {
            get
            {
                return xsb.is_nil(term) != 0;
            }
        }

        [ScriptMember("isCompound")]
        public bool IsCompound
        {
            get
            {
                return xsb.is_functor(term) != 0;
            }
        }

        [ScriptMember("isCharList")]
        public bool IsCharList
        {
            get
            {
                int list_size;
                return xsb.is_charlist(term, out list_size) != 0;
            }
        }

        [ScriptMember("isAttributedVariable")]
        public bool IsAttributedVariable
        {
            get
            {
                return xsb.is_attv(term) != 0;
            }
        }

        [ScriptMember("name")]
        public string Name
        {
            get
            {
                if (IsCompound)
                {
                    return xsb.p2c_functor(term);
                }
                else if (IsAtom)
                {
                    return xsb.p2c_string(term);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        [ScriptMember("arity")]
        public int Arity
        {
            get
            {
                if (IsCompound)
                {
                    return xsb.p2c_arity(term);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        public XsbPrologTerm this[int index]
        {
            get
            {
                return new XsbPrologTerm(xsb.p2p_arg(term, index));
            }
        }

        [ScriptMember("head")]
        public XsbPrologTerm Head
        {
            get
            {
                if (IsList)
                {
                    return new XsbPrologTerm(xsb.p2p_car(term));
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        [ScriptMember("tail")]
        public XsbPrologTerm Tail
        {
            get
            {
                if (IsList)
                {
                    return new XsbPrologTerm(xsb.p2p_cdr(term));
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        [ScriptMember("toInteger")]
        public int ToInteger()
        {
            return xsb.p2c_int(term);
        }

        [ScriptMember("toFloat")]
        public double ToFloat()
        {
            return xsb.p2c_float(term);
        }

        // void printpterm(prolog_term term)
        private string ToString(ulong term)
        {
            int a;
            int i;

            if (xsb.is_var(term) != 0) return "_" + term;
            else if (xsb.is_attv(term) != 0) return "_" + term;
            else if (xsb.is_int(term) != 0) return xsb.p2c_int(term).ToString();
            else if (xsb.is_float(term) != 0) return xsb.p2c_float(term).ToString();
            else if (xsb.is_nil(term) != 0) return "[]";
            else if (xsb.is_string(term) != 0) return xsb.p2c_string(term);
            else if (xsb.is_list(term) != 0)
            {
                string retval = "[";
                retval += ToString(xsb.p2p_car(term));
                term = xsb.p2p_cdr(term);
                while (xsb.is_list(term) != 0)
                {
                    retval += ",";
                    retval += ToString(xsb.p2p_car(term));
                    term = xsb.p2p_cdr(term);
                }
                if (xsb.is_nil(term) == 0)
                {
                    retval += "|";
                    retval += ToString(term);
                }
                retval += "]";
                return retval;
            }
            else if (xsb.is_functor(term) != 0)
            {
                string retval = xsb.p2c_functor(term);
                a = xsb.p2c_arity(term);
                if (a > 0)
                {
                    retval += "(";
                    retval += ToString(xsb.p2p_arg(term, 1));
                    for (i = 2; i <= a; i++)
                    {
                        retval += ",";
                        retval += ToString(xsb.p2p_arg(term, i));
                    }
                    retval += ")";
                }
                return retval;
            }
            else { throw new NotImplementedException(); }
        }

        [ScriptMember("toString")]
        public override string ToString()
        {
            return ToString(term);
        }
    }
}