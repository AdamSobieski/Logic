/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public static class Extensions
    {
        public static VariableExpression CreateVariable(this SetExpression set)
        {
            return Expression.Variable(set.Definition, set.Parameter);
        }
        public static VariableExpression AddConstraint(this VariableExpression variable, CompoundExpression constraint)
        {
            return Expression.Variable(variable.Constraints.Append(constraint), variable.Parameter);
        }
        public static VariableExpression RemoveConstraint(this VariableExpression variable, CompoundExpression constraint)
        {
            return Expression.Variable(variable.Constraints.Except(new CompoundExpression[] { constraint }), variable.Parameter);
        }

        public static bool CanUnify(this VariableExpression variable, Expression value, IContainer<CompoundExpression> kb)
        {
            if (!object.ReferenceEquals(variable, variable.Parameter))
                if (!variable.Parameter.CanUnify(value, kb))
                    return false;

            foreach (var constraint in variable.Constraints)
            {
                if (!kb.Contains(constraint.Replace(new Expression[] { variable.Parameter }, new Expression[] { value }) as CompoundExpression))
                    return false;
            }

            return true;
        }
        internal static bool CanUnify(this IEnumerable<VariableExpression> variables, IEnumerable<Expression> values, IContainer<CompoundExpression> kb)
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
                        if (!enumerator1.Current.CanUnify(enumerator2.Current, kb)) return false;

                        moveNext1 = enumerator1.MoveNext();
                        moveNext2 = enumerator2.MoveNext();

                        if (moveNext1 != moveNext2) return false;
                    }

                    return true;
                }
            }
        }

        public static bool IsValid(this PredicateExpression predicate, IEnumerable<Expression> arguments, IContainer<CompoundExpression> kb)
        {
            if (!predicate.Parameters.CanUnify(arguments, kb))
                return false;

            foreach (var condition in predicate.Preconditions)
            {
                if (!kb.Contains(condition.Replace(predicate.Parameters.ToArray(), arguments.ToArray()) as CompoundExpression))
                    return false;
            }

            return true;
        }
        public static bool IsValid(this CompoundExpression expression, IContainer<CompoundExpression> kb)
        {
            PredicateExpression predicate = expression.Predicate as PredicateExpression;
            if (predicate != null)
            {
                return predicate.IsValid(expression.Arguments, kb);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        public static bool IsValid(this IBinding binding, IContainer<CompoundExpression> kb)
        {
            return binding.Variables.CanUnify(binding.Bindings, kb);
        }

        public static bool Contains(this SetExpression set, Expression element, IContainer<CompoundExpression> kb)
        {
            if (!set.Parameter.CanUnify(element, kb))
                return false;

            foreach (var constraint in set.Definition)
            {
                if (!kb.Contains(constraint.Replace(new Expression[] { set.Parameter }, new Expression[] { element }) as CompoundExpression))
                    return false;
            }

            return true;
        }

        public static SetExpression Intersection(this SetExpression set, SetExpression other, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();
        }
        public static SetExpression Union(this SetExpression set, SetExpression other, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();
        }

        public static bool IsSubsetOf(this SetExpression set, SetExpression other, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();
        }
        public static bool IsSupersetOf(this SetExpression set, SetExpression other, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();
        }
    }
}
