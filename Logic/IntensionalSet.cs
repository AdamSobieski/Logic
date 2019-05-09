using Logic.Prolog.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Collections
{
    public class IntensionalSet
    {
        public string Name { get; }
        public IReadOnlyList<CompoundExpression> Definition { get; }
        public VariableExpression Parameter { get; }

        public VariableExpression CreateVariable(string name)
        {
            return Expression.Variable(name, Definition, Parameter);
        }
    }

    public static class Extensions
    {
        public static bool Contains(this IContainer<CompoundExpression> expressionSet, IntensionalSet set, Expression element)
        {
            foreach (var constraint in set.Definition)
            {
                if (!expressionSet.Contains(constraint.Replace(new Expression[] { set.Parameter }, new Expression[] { element }) as CompoundExpression))
                    return false;
            }
            return true;
        }
        public static bool Contains(this IntensionalSet set, Expression element, IContainer<CompoundExpression> expressionSet)
        {
            foreach (var constraint in set.Definition)
            {
                if (!expressionSet.Contains(constraint.Replace(new Expression[] { set.Parameter }, new Expression[] { element }) as CompoundExpression))
                    return false;
            }
            return true;
        }

        public static bool IsValid(this IContainer<CompoundExpression> expressionSet, PredicateExpression predicate, IReadOnlyList<Expression> arguments)
        {
            if (predicate.Parameters.Count != arguments.Count) return false;

            foreach (var condition in predicate.Preconditions)
            {
                if (!expressionSet.Contains(condition.Replace(predicate.Parameters.ToArray(), arguments.ToArray()) as CompoundExpression))
                    return false;
            }
            return true;
        }
        public static bool IsValid(this PredicateExpression predicate, IReadOnlyList<Expression> arguments, IContainer<CompoundExpression> expressionSet)
        {
            if (predicate.Parameters.Count != arguments.Count) return false;

            foreach (var condition in predicate.Preconditions)
            {
                if (!expressionSet.Contains(condition.Replace(predicate.Parameters.ToArray(), arguments.ToArray()) as CompoundExpression))
                    return false;
            }
            return true;
        }

        public static bool IsValid(this IContainer<CompoundExpression> expressionSet, CompoundExpression expression)
        {
            PredicateExpression predicate = expression.Predicate as PredicateExpression;
            if (predicate != null)
            {
                return expressionSet.IsValid(predicate, expression.Arguments);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static bool IsValid(this CompoundExpression expression, IContainer<CompoundExpression> expressionSet)
        {
            PredicateExpression predicate = expression.Predicate as PredicateExpression;
            if (predicate != null)
            {
                return expressionSet.IsValid(predicate, expression.Arguments);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static bool IsSubsetOf(this IntensionalSet set, IntensionalSet other, IContainer<CompoundExpression> expressionSet)
        {
            throw new NotImplementedException();
        }
        public static bool IsSupersetOf(this IntensionalSet set, IntensionalSet other, IContainer<CompoundExpression> expressionSet)
        {
            throw new NotImplementedException();
        }
    }
}
