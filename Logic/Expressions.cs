using System;
using System.Collections.Generic;

namespace Logic.Expressions
{
    public abstract class Expression
    {
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
        public static PredicateExpression Predicate(string module, string name, int arity, IEnumerable<Expression> preconditions, IEnumerable<VariableExpression> parameters)
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

        public static LambdaExpression Lambda(string name, IEnumerable<Expression> preconditions, Expression body, IEnumerable<Expression> postconditions, IEnumerable<VariableExpression> parameters)
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
        public IReadOnlyList<Expression> Preconditions { get; }
    }

    public class CompoundExpression : Expression
    {
        public new Expression Predicate { get; }
        public IReadOnlyList<Expression> Arguments { get; }
    }

    public class LambdaExpression : Expression
    {
        public IReadOnlyList<VariableExpression> Parameters { get; }
        public IReadOnlyList<Expression> Preconditions { get; }
        public Expression Body { get; }
        public IReadOnlyList<Expression> Postconditions { get; }

        public LambdaExpression Reparameterize(params VariableExpression[] parameters)
        {
            throw new NotImplementedException();
        }
        public LambdaExpression Reparameterize(IEnumerable<VariableExpression> parameters)
        {
            throw new NotImplementedException();
        }
    }
}