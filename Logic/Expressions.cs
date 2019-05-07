using Logic.Collections;
using Logic.Incremental;
using Logic.Planning;
using System;
using System.Collections.Generic;

namespace Logic.Expressions
{
    public abstract class Expression
    {
        public static Expression Constant(int value)
        {
            throw new NotImplementedException();
        }
        public static Expression Constant(double value)
        {
            throw new NotImplementedException();
        }
        public static Expression Constant(object value)
        {
            throw new NotImplementedException();
        }

        public static VariableExpression Variable(string name)
        {
            throw new NotImplementedException();
        }
        public static VariableExpression Variable(string name, params Expression[] constraints)
        {
            throw new NotImplementedException();
        }
        public static VariableExpression Variable(string name, IEnumerable<Expression> constraints)
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



        public virtual Expression Replace(VariableExpression[] from, Expression[] to)
        {
            throw new NotImplementedException();
        }
    }

    public class VariableExpression : Expression
    {
        public IReadOnlyList<Expression> Constraints { get; }

        public VariableExpression AddConstraint(Expression constraint)
        {
            throw new NotImplementedException();
        }
        public VariableExpression RemoveConstraint(Expression constraint)
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
    }

    public class CompoundExpression : Expression
    {
        public new Expression Predicate { get; }
        public IReadOnlyList<Expression> Arguments { get; }
    }

    public class LambdaExpression : Expression, IAction
    {
        public string Module { get; }
        public string Name { get; }
        public IReadOnlyList<VariableExpression> Parameters { get; }
        public ICompoundExpressionList Preconditions { get; }
        public Expression Body { get; }
        public ICompoundExpressionDelta Effects { get; }

        //public LambdaExpression Reparameterize(params VariableExpression[] parameters)
        //{
        //    throw new NotImplementedException();
        //}
        //public LambdaExpression Reparameterize(IEnumerable<VariableExpression> parameters)
        //{
        //    throw new NotImplementedException();
        //}
    }
}