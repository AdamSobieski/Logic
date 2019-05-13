﻿/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public static class Extensions
    {
        public static VariableExpression CreateVariable(this SetExpression set)
        {
            return Expression.Variable(set.Parameters, set.Constraints);
        }
        public static VariableExpression AddConstraint(this VariableExpression variable, CompoundExpression constraint)
        {
            return Expression.Variable(variable.Parameters, variable.Constraints.Append(constraint));
        }
        public static VariableExpression RemoveConstraint(this VariableExpression variable, CompoundExpression constraint)
        {
            return Expression.Variable(variable.Parameters, variable.Constraints.Except(new CompoundExpression[] { constraint }));
        }

        public static bool CanUnify(this VariableExpression variable, IEnumerable<Expression> value, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();
            //if (!object.ReferenceEquals(variable, variable.Parameter))
            //    if (!variable.Parameter.CanUnify(value, kb))
            //        return false;

            //foreach (var constraint in variable.Constraints)
            //{
            //    if (!kb.Contains(constraint.Replace(new Expression[] { variable.Parameter }, new Expression[] { value }) as CompoundExpression))
            //        return false;
            //}

            //return true;
        }
        internal static bool CanUnify(this IEnumerable<VariableExpression> variables, IEnumerable<IEnumerable<Expression>> values, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();
            //using (var enumerator1 = variables.GetEnumerator())
            //{
            //    using (var enumerator2 = values.GetEnumerator())
            //    {
            //        bool moveNext1, moveNext2;

            //        moveNext1 = enumerator1.MoveNext();
            //        moveNext2 = enumerator2.MoveNext();

            //        if (moveNext1 != moveNext2) return false;

            //        while (moveNext1)
            //        {
            //            if (!enumerator1.Current.CanUnify(enumerator2.Current, kb)) return false;

            //            moveNext1 = enumerator1.MoveNext();
            //            moveNext2 = enumerator2.MoveNext();

            //            if (moveNext1 != moveNext2) return false;
            //        }

            //        return true;
            //    }
            //}
        }

        public static bool IsValid(this PredicateExpression predicate, IEnumerable<Expression> arguments, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();

            //if (!predicate.Parameters.CanUnify(arguments, kb))
            //    return false;

            //foreach (var condition in predicate.Constraints)
            //{
            //    if (!kb.Contains(condition.Replace(predicate.Parameters.ToArray(), arguments.ToArray()) as CompoundExpression))
            //        return false;
            //}

            //return true;
        }
        public static bool IsValid(this CompoundExpression expression, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();

            //PredicateExpression predicate = expression.Predicate as PredicateExpression;
            //if (predicate != null)
            //{
            //    return predicate.IsValid(expression.Arguments, kb);
            //}
            //else
            //{
            //    throw new NotImplementedException();
            //}
        }
        public static bool IsValid(this IBinding binding, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();
            // return binding.Variables.CanUnify(binding.Bindings, kb);
        }

        public static bool Contains(this SetExpression set, Expression element, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();

            //if (!set.Parameters.CanUnify(element, kb))
            //    return false;

            //foreach (var constraint in set.Definition)
            //{
            //    if (!kb.Contains(constraint.Replace(new Expression[] { set.Parameters }, new Expression[] { element }) as CompoundExpression))
            //        return false;
            //}

            //return true;
        }

        public static SetExpression Intersection(this SetExpression set, SetExpression other, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();
        }
        public static SetExpression Union(this SetExpression set, SetExpression other, IContainer<CompoundExpression> kb)
        {
            throw new NotImplementedException();
        }

        public static SetExpression CartesianProduct(this SetExpression set, SetExpression other)
        {
            // or flatten both parameter lists into higher-dimensional variables and then concat the two higher-dimensional variables?
            return Expression.Set(set.Parameters.Concat(other.Parameters), set.Constraints.Concat(other.Constraints));
        }
        public static int Dimensionality(this SetExpression set)
        {
            return set.Parameters.Count;
        }
        public static int Dimensionality(this VariableExpression set)
        {
            return set.Parameters.Count;
        }
        public static int Arity(this PredicateExpression predicate)
        {
            return predicate.Parameters.Count;
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