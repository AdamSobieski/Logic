using Logic.Expressions;
using Logic.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Logic.Explanation
{
    public enum JustificationSettings
    {
        Optimize = 0,
        Compounds = 1,
        Justifications = 2
    }

    public sealed class Justification : IEquatable<Justification>
    {
        static ReadOnlyCollection<RuleApplication> s_empty = new ReadOnlyCollection<RuleApplication>(new List<RuleApplication>(0));

        public Justification(EqualExpression expression)
        {
            m_expr = expression;
            m_ras = s_empty;
        }
        public Justification(EqualExpression expression, RuleApplication ra)
        {
            m_expr = expression;
            var list = new List<RuleApplication>(1);
            list.Add(ra);
            m_ras = new ReadOnlyCollection<RuleApplication>(list);
        }
        public Justification(EqualExpression expression, IEnumerable<RuleApplication> ras)
        {
            m_expr = expression;
            var list = new List<RuleApplication>(ras);
            list.TrimExcess();
            m_ras = new ReadOnlyCollection<RuleApplication>(list);
        }

        readonly EqualExpression m_expr;
        readonly ReadOnlyCollection<RuleApplication> m_ras;

        public EqualExpression Expression
        {
            get
            {
                return m_expr;
            }
        }
        public IReadOnlyList<RuleApplication> Derivations
        {
            get
            {
                return m_ras;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Justification j)
            {
                return Equals(j);
            }
            else
            {
                return false;
            }
        }
        public bool Equals(Justification other)
        {
            if (!m_expr.Equals(other.m_expr)) return false;
            int count = m_ras.Count;
            if (count != other.m_ras.Count) return false;
            for (int i = 0; i < count; ++i)
            {
                if (!m_ras[i].Equals(other.m_ras[i])) return false;
            }
            return true;
        }
    }

    public sealed class RuleApplication : IEquatable<RuleApplication>
    {
        public RuleApplication(RuleInfo rule, IEnumerable<KeyValuePair<EqualExpression, Justification>> data)
        {
            m_rule = rule;
            var dictionary = new Dictionary<EqualExpression, Justification>();
            foreach (var kvp in data)
                dictionary.Add(kvp.Key, kvp.Value);
            m_data = new ReadOnlyDictionary<EqualExpression, Justification>(dictionary);

            // can generate m_bindings automatically
        }

        readonly RuleInfo m_rule;
        readonly ReadOnlyDictionary<VariableExpression, ConstantExpression> m_bindings;
        readonly ReadOnlyDictionary<EqualExpression, Justification> m_data;

        public RuleInfo Rule
        {
            get
            {
                return m_rule;
            }
        }
        public IReadOnlyDictionary<VariableExpression, ConstantExpression> Bindings
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        public IReadOnlyDictionary<EqualExpression, Justification> Dependencies
        {
            get
            {
                return m_data;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is RuleApplication ra)
            {
                return Equals(ra);
            }
            else
            {
                return false;
            }
        }
        public bool Equals(RuleApplication other)
        {
            if (!m_rule.Equals(other.m_rule)) return false;
            if (m_data.Count != other.m_data.Count) return false;
            foreach (var kvp in m_data)
            {
                Justification otherValue;
                if (!other.m_data.TryGetValue(kvp.Key, out otherValue)) return false;
                if (!kvp.Value.Equals(otherValue)) return false;
            }
            return true;
        }
    }
}