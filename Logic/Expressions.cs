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
        public static VariableExpression Variable(string name, params LambdaExpression[] constraints)
        {
            throw new NotImplementedException();
        }
        public static VariableExpression Variable(string name, IEnumerable<LambdaExpression> constraints)
        {
            throw new NotImplementedException();
        }

        public static PredicateExpression Predicate(string module, string name, int arity)
        {
            throw new NotImplementedException();
        }
        public static PredicateExpression Predicate(string module, string name, int arity, IEnumerable<VariableExpression> parameters)
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
        public IReadOnlyList<Expression> Constraints { get; }

        public VariableExpression AddConstraint(LambdaExpression constraint)
        {
            throw new NotImplementedException();
        }
        public VariableExpression RemoveConstraint(LambdaExpression constraint)
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
            throw new NotImplementedException();
        }
    }

    public class CompoundExpression : Expression
    {
        public new Expression Predicate { get; }
        public IReadOnlyList<Expression> Arguments { get; }

        internal override Expression Replace(Expression[] from, Expression[] to)
        {
            bool any = false;
            Expression p = Predicate;
            Expression p2 = p.Replace(from, to);
            Expression a;
            Expression a2;

            if (!object.ReferenceEquals(p, p2))
            {
                any = true;
            }
            int count = Arguments.Count;
            List<Expression> args = new List<Expression>(count);
            for (int x = 0; x < count; ++x)
            {
                a = Arguments[x];
                a2 = a.Replace(from, to);
                if (!object.ReferenceEquals(a, a2))
                {
                    any = true;
                }
                args.Add(a2);
            }
            if (!any) return this;
            else return Expression.Compound(p2, args);
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