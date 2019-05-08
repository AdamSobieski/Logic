using Logic.Planning;
using Logic.Prolog.Collections;
using Logic.Prolog.Incremental;
using Logic.Prolog.Knowledge;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Logic.Prolog.Expressions
{
    internal struct Atom
    {
        public Atom(string name)
        {
            m_name = name;
        }

        string m_name;

        public string Name
        {
            get
            {
                return m_name;
            }
        }
    }

    public abstract class Expression
    {
        public static ConstantExpression Constant(object value)
        {
            return new ConstantExpression(value, value != null ? value.GetType() : typeof(object));
        }
        public static ConstantExpression Constant(object value, Type type)
        {
            Contract.Requires(type != null);
            if (value == null && type.IsValueType && !(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                throw new ArgumentException();
            }
            if (value != null && !type.IsAssignableFrom(value.GetType()))
            {
                throw new ArgumentException();
            }
            return new ConstantExpression(value, type);
        }

        public static ConstantExpression Integer(int value)
        {
            return new ConstantExpression(value, typeof(int));
        }
        public static ConstantExpression Float(double value)
        {
            return new ConstantExpression(value, typeof(double));
        }
        public static ConstantExpression Atom(string name)
        {
            return new ConstantExpression(new Atom(name), typeof(Atom));
        }

        public static VariableExpression Variable(string name)
        {
            throw new NotImplementedException();
        }
        public static VariableExpression Variable(string name, IEnumerable<CompoundExpression> constraints, VariableExpression parameter)
        {
            throw new NotImplementedException();
        }

        public static PredicateExpression Predicate(string module, string name, int arity)
        {
            throw new NotImplementedException();
        }
        public static PredicateExpression Predicate(string module, string name, int arity, IEnumerable<CompoundExpression> preconditions, IEnumerable<VariableExpression> parameters)
        {
            throw new NotImplementedException();
        }

        public static CompoundExpression Compound(Expression predicate, params Expression[] arguments)
        {
            throw new NotImplementedException();
        }
        public static CompoundExpression Compound(Expression predicate, IEnumerable<Expression> arguments)
        {
            throw new NotImplementedException();
        }

        public static Expression Evaluate(Expression /*ICompoundExpressionContainer*/ set, CompoundExpression expression)
        {
            throw new NotImplementedException();
        }

        public static LambdaExpression Lambda(string module, string name, IEnumerable<CompoundExpression> preconditions, Expression body, ICompoundExpressionDelta effects, IEnumerable<VariableExpression> parameters)
        {
            throw new NotImplementedException();
        }



        public virtual Type Type
        {
            get
            {
                return typeof(object);
            }
        }



        internal virtual Expression Replace(Expression[] from, Expression[] to)
        {
            int index = Array.IndexOf(from, this);
            return (index >= 0) ? to[index] : this;
        }
    }

    public class ConstantExpression : Expression
    {
        internal ConstantExpression(object value, Type type)
        {
            m_value = value;
            m_type = type;
        }
        object m_value;
        Type m_type;

        public object Value => m_value;
        public override Type Type => m_type;
    }

    public class VariableExpression : Expression
    {
        public string Name { get; }

        public ICompoundExpressionList Constraints { get; }

        public VariableExpression AddConstraint(CompoundExpression constraint)
        {
            throw new NotImplementedException();
        }
        public VariableExpression RemoveConstraint(CompoundExpression constraint)
        {
            throw new NotImplementedException();
        }
    }

    public class PredicateExpression : Expression
    {
        public string Module { get; }
        public string Name { get; }
        public int Arity { get; }

        public IReadOnlyList<VariableExpression> Parameters { get; }
        public ICompoundExpressionList Preconditions { get; }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            bool any = false;
            Expression e;
            Expression n;
            List<VariableExpression> p;
            List<CompoundExpression> c;
            int index;
            int count;

            index = Array.IndexOf(from, this);
            if (index >= 0) return to[index];

            count = Parameters.Count;
            p = new List<VariableExpression>(count);

            for (index = 0; index < count; index++)
            {
                e = Parameters[index];
                n = e.Replace(from, to);
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                p.Add((VariableExpression)n);
            }

            count = Preconditions.Count;
            c = new List<CompoundExpression>(count);

            for (index = 0; index < count; index++)
            {
                e = Preconditions[index];
                n = e.Replace(from, to);
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                c.Add((CompoundExpression)n);
            }

            if (!any) return this;
            return Expression.Predicate(Module, Name, Arity, c, p);
        }

        public bool IsValid(IKnowledgebaseModule module, IReadOnlyList<Expression> arguments)
        {
            if (Parameters.Count != arguments.Count) return false;

            foreach (var condition in Preconditions)
            {
                if (!module.Contains(condition.Replace(Parameters.ToArray(), arguments.ToArray()) as CompoundExpression))
                    return false;
            }
            return true;
        }
    }

    public class CompoundExpression : Expression
    {
        public new Expression Predicate { get; }
        public IReadOnlyList<Expression> Arguments { get; }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            bool any = false;
            Expression e;
            Expression n;
            Expression p;
            List<Expression> args;
            int index;
            int count;

            index = Array.IndexOf(from, this);
            if (index >= 0) return to[index];

            e = Predicate;
            p = e.Replace(from, to);

            if (!object.ReferenceEquals(e, p))
            {
                any = true;
            }

            count = Arguments.Count;
            args = new List<Expression>(count);
            for (index = 0; index < count; ++index)
            {
                e = Arguments[index];
                n = e.Replace(from, to);
                if (!object.ReferenceEquals(e, n))
                {
                    any = true;
                }
                args.Add(n);
            }
            if (!any) return this;
            else return Expression.Compound(p, args);
        }

        public bool IsValid(IKnowledgebaseModule module)
        {
            PredicateExpression p = Predicate as PredicateExpression;
            if (p != null)
            {
                return p.IsValid(module, Arguments);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    public class LambdaExpression : Expression, IAction
    {
        public string Module { get; }
        public string Name { get; }

        public IReadOnlyList<VariableExpression> Parameters { get; }
        public ICompoundExpressionList Preconditions { get; }
        public Expression Body { get; }
        public ICompoundExpressionDelta Effects { get; }

        public Type ReturnType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            throw new NotImplementedException();
        }
    }
}