using Logic.Collections;
using Logic.Incremental;
using Logic.Planning;
using System;
using System.Collections.Generic;

namespace Logic.Expressions
{
    public abstract class Expression
    {
        public static ConstantExpression Constant(object value)
        {
            throw new NotImplementedException();
        }
        public static ConstantExpression Constant(object value, Type type)
        {
            throw new NotImplementedException();
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

    }

    public class VariableExpression : Expression
    {
        public string Name { get; }

        public IReadOnlyList<CompoundExpression> Constraints { get; }

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