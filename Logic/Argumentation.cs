/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Expressions;
using System.Collections.Generic;

namespace Logic.Argumentation
{
    public interface _1
    {
        CompoundExpression Rule { get; }
        IBinding Binding { get; }
        CompoundExpression Expression { get; }
    }

    public interface _2
    {
        IReadOnlyList<CompoundExpression> RuleSupport { get; }
        IReadOnlyList<CompoundExpression> RuleDerived { get; }
        IBinding Binding { get; }
        IReadOnlyList<CompoundExpression> Support { get; }
        IReadOnlyList<CompoundExpression> Derived { get; }
    }

    public interface _3
    {
        IReadOnlyList<CompoundExpression> RuleSupport { get; }
        CompoundExpression RuleDerived { get; }
        IBinding Binding { get; }
        IReadOnlyList<CompoundExpression> Support { get; }
        CompoundExpression Derived { get; }
    }

    public interface _4
    {
        IReadOnlyList<CompoundExpression> RuleSupport { get; }
        CompoundExpression RuleDerived { get; }
        IBinding Binding { get; }
        IReadOnlyList<_4> Support { get; }
        CompoundExpression Derived { get; }
    }

    public interface _5
    {
        CompoundExpression Expression { get; }
        IReadOnlyList<_6> SupportedBy { get; }
        IReadOnlyList<_6> OpposedBy { get; }
    }

    public interface _6
    {
        IReadOnlyList<CompoundExpression> RuleSupport { get; }
        CompoundExpression RuleDerived { get; }
        IBinding Binding { get; }
        IReadOnlyList<_5> Support { get; }
        _5 Derived { get; }
    }
}