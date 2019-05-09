using Logic.Collections;
using Logic.Prolog.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public static class Extensions
    {
        public static VariableExpression CreateVariable(this IntensionalSet set)
        {
            return Expression.Variable(set.Definition, set.Parameter);
        }
        public static VariableExpression AddConstraint(this VariableExpression variable, CompoundExpression constraint)
        {
            return Expression.Variable(variable.Constraints.Append(constraint), variable);
        }
        public static VariableExpression RemoveConstraint(this VariableExpression variable, CompoundExpression constraint)
        {
            return Expression.Variable(variable.Constraints.Except(new CompoundExpression[] { constraint }), variable);
        }

        public static bool CanUnify(this VariableExpression variable, Expression value, IContainer<CompoundExpression> expressionSet)
        {
            if (variable.Parameter != null)
                if (!variable.Parameter.CanUnify(value, expressionSet))
                    return false;

            foreach (var constraint in variable.Constraints)
            {
                if (!expressionSet.Contains(constraint.Replace(new Expression[] { variable.Parameter }, new Expression[] { value }) as CompoundExpression))
                    return false;
            }

            return true;
        }
        internal static bool CanUnify(this IEnumerable<VariableExpression> variables, IEnumerable<Expression> values, IContainer<CompoundExpression> expressionSet)
        {
            using (var enumerator1 = variables.GetEnumerator())
            {
                using (var enumerator2 = values.GetEnumerator())
                {
                    bool moveNext1, moveNext2;

                    moveNext1 = enumerator1.MoveNext();
                    moveNext2 = enumerator2.MoveNext();

                    if (moveNext1 != moveNext2) return false;

                    while (moveNext1)
                    {
                        if (!enumerator1.Current.CanUnify(enumerator2.Current, expressionSet)) return false;

                        moveNext1 = enumerator1.MoveNext();
                        moveNext2 = enumerator2.MoveNext();

                        if (moveNext1 != moveNext2) return false;
                    }

                    return true;
                }
            }
        }

        public static bool IsValid(this PredicateExpression predicate, IEnumerable<Expression> arguments, IContainer<CompoundExpression> expressionSet)
        {
            if (!predicate.Parameters.CanUnify(arguments, expressionSet))
                return false;

            foreach (var condition in predicate.Preconditions)
            {
                if (!expressionSet.Contains(condition.Replace(predicate.Parameters.ToArray(), arguments.ToArray()) as CompoundExpression))
                    return false;
            }
            return true;
        }
        public static bool IsValid(this CompoundExpression expression, IContainer<CompoundExpression> expressionSet)
        {
            PredicateExpression predicate = expression.Predicate as PredicateExpression;
            if (predicate != null)
            {
                return predicate.IsValid(expression.Arguments, expressionSet);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static bool Contains(this IntensionalSet set, Expression element, IContainer<CompoundExpression> expressionSet)
        {
            if (set.Parameter != null)
                if (!set.Parameter.CanUnify(element, expressionSet))
                    return false;

            foreach (var constraint in set.Definition)
            {
                if (!expressionSet.Contains(constraint.Replace(new Expression[] { set.Parameter }, new Expression[] { element }) as CompoundExpression))
                    return false;
            }

            return true;
        }

        public static IntensionalSet Intersection(this IntensionalSet set, IntensionalSet other, IContainer<CompoundExpression> expressionSet)
        {
            throw new NotImplementedException();
        }
        public static IntensionalSet Union(this IntensionalSet set, IntensionalSet other, IContainer<CompoundExpression> expressionSet)
        {
            throw new NotImplementedException();
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
