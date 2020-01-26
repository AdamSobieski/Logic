using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public static class Extensions
    {
        internal struct EmptyScope : IDisposable
        {
            public void Dispose() { }
        }
        internal static IDisposable s_emptyScope = new EmptyScope();
        public static IDisposable Unify(this object obj, object arg, out bool unified)
        {
            obj = (obj is Variable x1) ? x1.Value : obj;
            arg = (arg is Variable x2) ? x2.Value : arg;

            if (obj is IUnifiable u1)
            {
                return u1.Unify(arg, out unified);
            }
            else if (arg is IUnifiable u2)
            {
                return u2.Unify(obj, out unified);
            }
            else
            {
                if (obj == null)
                {
                    if (arg == null)
                    {
                        unified = true;
                        return s_emptyScope;
                    }
                    else
                    {
                        unified = false;
                        return s_emptyScope;
                    }
                }
                else if (obj.Equals(arg))
                {
                    unified = true;
                    return s_emptyScope;
                }
                else
                {
                    unified = false;
                    return s_emptyScope;
                }
            }
        }
    }

    public interface IUnifiable
    {
        IDisposable Unify(object arg, out bool unified);
    }

    public delegate bool UnificationConstraint(object arg);

    public sealed class Variable : IUnifiable
    {
        private struct Scope : IDisposable
        {
            public Scope(Variable x)
            {
                m_x = x;
            }
            Variable m_x;

            public void Dispose()
            {
                m_x.m_isBound = false;
                m_x = null;
            }
        }

        public Variable()
        {
            m_isBound = false;
            m_constraints = null;
        }
        public Variable(params UnificationConstraint[] constraints)
        {
            m_isBound = false;
            m_constraints = new List<UnificationConstraint>(constraints);
        }
        public Variable(IEnumerable<UnificationConstraint> constraints)
        {
            m_isBound = false;
            m_constraints = new List<UnificationConstraint>(constraints);
        }

        private bool m_isBound;
        private object m_value;
        private List<UnificationConstraint> m_constraints;

        public bool IsBound => m_isBound;
        public bool HasConstraints => m_constraints != null;

        public object Value
        {
            get
            {
                if (!m_isBound)
                    return this;

                object result = m_value;
                while (result is Variable x)
                {
                    if (!x.m_isBound)
                        return result;

                    result = x.m_value;
                }

                return result;
            }
        }

        public IDisposable Unify(object arg, out bool unified)
        {
            if (!m_isBound)
            {
                object _arg = (arg is Variable x) ? x.Value : arg;

                if (m_constraints != null)
                {
                    bool all = true;
                    for (int i = 0; i < m_constraints.Count; ++i)
                    {
                        if (!m_constraints[i](_arg))
                        {
                            all = false;
                            break;
                        }
                    }
                    if (!all)
                    {
                        unified = false;
                        return Extensions.s_emptyScope;
                    }
                }

                m_value = _arg;
                if (m_value == this)
                {
                    unified = true;
                    return Extensions.s_emptyScope;
                }
                else
                {
                    m_isBound = true;
                    unified = true;
                    return new Scope(this);
                }
            }
            else
            {
                return Extensions.Unify(this, arg, out unified);
            }
        }
    }
}