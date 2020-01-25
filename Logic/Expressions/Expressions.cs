using Logic.Collections;
using Logic.Explanation;
using Logic.Reflection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Logic.Expressions
{
    public enum ExpressionType
    {
        Constant,
        Variable,
        Compound,
        Equal,
        And,
        Or,
        Rule
    }

    public abstract class Expression
    {
        public static ConstantExpression Constant(object value)
        {
            return new ConstantExpression(value, value != null ? value.GetType() : typeof(object));
        }
        public static ConstantExpression Constant(object value, Type type)
        {
            return new ConstantExpression(value, type);
        }
        public static VariableExpression Variable(string name, Type type)
        {
            return new VariableExpression(name, type);
        }
        public static CompoundExpression Compound(FunctorInfo functor, params Expression[] arguments)
        {
            return new CompoundExpression(functor, arguments);
        }
        public static EqualExpression Equal(CompoundExpression compound, ConstantExpression value)
        {
            return new EqualExpression(compound, value);
        }
        public static EqualExpression Equal(FunctorInfo functor, object[] data)
        {
            int length = data.Length;
            ConstantExpression[] expressions = new ConstantExpression[length - 1];
            for (int i = 1; i < length; ++i)
            {
                object data_i = data[i];
                expressions[i - 1] = Constant((data_i is Variable x) ? x.Value : data_i);
            }
            ConstantExpression value = Constant(data[0]);
            CompoundExpression compound = Compound(functor, expressions);
            return Equal(compound, value);
        }
        public static EqualExpression Equal(FunctorInfo functor, object value, object[] tuple)
        {
            int length = tuple.Length;
            ConstantExpression[] expressions = new ConstantExpression[length];
            for (int i = 0; i < length; ++i)
            {
                object tuple_i = tuple[i];
                expressions[i] = Constant((tuple_i is Variable x1) ? x1.Value : tuple_i);
            }
            ConstantExpression _value = Constant((value is Variable x2) ? x2.Value : value);
            CompoundExpression compound = Compound(functor, expressions);
            return Equal(compound, _value);
        }
        public static AndExpression And(params Expression[] expressions)
        {
            if (expressions.Length < 2) throw new ArgumentException("", nameof(expressions));
            return new AndExpression(expressions);
        }
        public static OrExpression Or(params Expression[] expressions)
        {
            if (expressions.Length < 2) throw new ArgumentException("", nameof(expressions));
            return new OrExpression(expressions);
        }
        public static RuleExpression Rule(EqualExpression head, Expression body)
        {
            return new RuleExpression(head, body);
        }

        public abstract ExpressionType ExpressionType { get; }
        public abstract Type Type { get; }
    }

    public abstract class TermExpression : Expression { }

    public sealed class ConstantExpression : TermExpression
    {
        internal ConstantExpression(object value, Type type)
        {
            m_value = value;
            m_type = type;
        }
        readonly object m_value;
        readonly Type m_type;

        public override Type Type => m_type;
        public override ExpressionType ExpressionType => ExpressionType.Constant;

        public object Value => m_value;

        public override bool Equals(object obj)
        {
            if (obj is ConstantExpression other)
            {
                return m_value.Equals(other.m_value) && m_type.Equals(other.m_type);
            }
            else
            {
                return false;
            }
        }
    }

    public sealed class VariableExpression : TermExpression
    {
        internal VariableExpression(string name, Type type)
        {
            m_name = name;
            m_type = type;
        }
        readonly string m_name;
        readonly Type m_type;

        public string Name => m_name;
        public override Type Type => m_type;
        public override ExpressionType ExpressionType => ExpressionType.Variable;

        public override bool Equals(object obj)
        {
            if (obj is VariableExpression other)
            {
                return m_name.Equals(other.m_name) && m_type.Equals(other.m_type);
            }
            else
            {
                return false;
            }
        }
    }

    public sealed class CompoundExpression : Expression
    {
        internal CompoundExpression(FunctorInfo functor, Expression[] arguments)
        {
            m_functor = functor;
            int length = arguments.Length;
            var parameters = functor.GetParameters();
            var params_length = parameters.Length;
            if (params_length != length) throw new ArgumentException("", nameof(arguments));
            var list = new List<Expression>(length);
            for (int i = 0; i < length; ++i)
            {
                var argument = arguments[i];
                if (!parameters[i].ParameterType.IsAssignableFrom(argument.Type)) throw new ArgumentException("", nameof(arguments));
                list.Add(argument);
            }
            m_arguments = new ReadOnlyCollection<Expression>(list);
        }
        readonly FunctorInfo m_functor;
        readonly ReadOnlyCollection<Expression> m_arguments;

        public FunctorInfo Functor => m_functor;
        public ReadOnlyCollection<Expression> Arguments => m_arguments;
        public override Type Type => m_functor.FunctorType;
        public override ExpressionType ExpressionType => ExpressionType.Compound;

        public override bool Equals(object obj)
        {
            if (obj is CompoundExpression other)
            {
                if (!m_functor.Equals(other.m_functor)) return false;
                int count = m_arguments.Count;
                for (int i = 0; i < count; ++i)
                {
                    if (!m_arguments[i].Equals(other.m_arguments[i])) return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public sealed class EqualExpression : Expression
    {
        internal EqualExpression(Expression left, Expression right)
        {
            m_left = left;
            m_right = right;
        }
        readonly Expression m_left;
        readonly Expression m_right;

        public Expression Left => m_left;
        public Expression Right => m_right;

        public override Type Type => typeof(bool);
        public override ExpressionType ExpressionType => ExpressionType.Equal;

        public override bool Equals(object obj)
        {
            if (obj is EqualExpression other)
            {
                return m_left.Equals(other.m_left) && m_right.Equals(other.m_right);
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<Justification> Evaluate(IKnowledgebase kb, RuleSettings settings)
        {
            CompoundExpression compound = m_left as CompoundExpression;
            var args = compound.Arguments;
            var count = args.Count;
            object[] pattern = new object[count + 1];
            for (int i = 0; i < count; ++i)
            {
                pattern[i + 1] = (args[i] as ConstantExpression).Value;
            }
            pattern[0] = (m_right as ConstantExpression).Value;

            return kb.Match(compound.Functor.DeclaringType.FullName + "." + compound.Functor.Name + ", " + compound.Functor.DeclaringType.Assembly.FullName, Mode.StoredAndDerivedAdditions, pattern, settings);
        }
    }

    public sealed class AndExpression : Expression
    {
        internal AndExpression(Expression[] expressions)
        {
            int length = expressions.Length;
            for (int i = 0; i < length; ++i)
                if (expressions[i].Type != typeof(bool)) throw new ArgumentException("", nameof(expressions));
            var list = new List<Expression>(length);
            list.AddRange(expressions);
            m_children = new ReadOnlyCollection<Expression>(list);
        }
        ReadOnlyCollection<Expression> m_children;

        public ReadOnlyCollection<Expression> Children => m_children;
        public override Type Type => typeof(bool);
        public override ExpressionType ExpressionType => ExpressionType.And;
    }

    public sealed class OrExpression : Expression
    {
        internal OrExpression(Expression[] expressions)
        {
            int length = expressions.Length;
            for (int i = 0; i < length; ++i)
                if (expressions[i].Type != typeof(bool)) throw new ArgumentException("", nameof(expressions));
            var list = new List<Expression>(length);
            list.AddRange(expressions);
            m_children = new ReadOnlyCollection<Expression>(list);
        }
        ReadOnlyCollection<Expression> m_children;

        public ReadOnlyCollection<Expression> Children => m_children;
        public override Type Type => typeof(bool);
        public override ExpressionType ExpressionType => ExpressionType.Or;
    }

    public sealed class RuleExpression : Expression
    {
        internal RuleExpression(EqualExpression head, Expression body)
        {
            m_head = head;
            m_body = body;
        }

        readonly EqualExpression m_head;
        readonly Expression m_body;

        public EqualExpression Head => m_head;
        public Expression Body => m_body;

        public override Type Type => typeof(void);
        public override ExpressionType ExpressionType => ExpressionType.Rule;
    }
}