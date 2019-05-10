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
        IReadOnlyList<CompoundExpression> RuleSupport { get; }
        IReadOnlyList<CompoundExpression> RuleDerived { get; }
        IBinding Binding { get; }
        IReadOnlyList<CompoundExpression> Support { get; }
        IReadOnlyList<CompoundExpression> Derived { get; }
    }

    public interface _2
    {
        IReadOnlyList<CompoundExpression> RuleSupport { get; }
        CompoundExpression RuleDerived { get; }
        IBinding Binding { get; }
        IReadOnlyList<CompoundExpression> Support { get; }
        CompoundExpression Derived { get; }
    }

    public interface _3
    {
        CompoundExpression Rule { get; }
        IBinding Binding { get; }
        CompoundExpression Expression { get; }
    }

    public interface _4
    {
        IReadOnlyList<CompoundExpression> Derived { get; }
        IReadOnlyList<_4> Support { get; }
        IBinding Binding { get; }
        IReadOnlyList<CompoundExpression> RuleDerived { get; }
        IReadOnlyList<CompoundExpression> RuleSupport { get; }
    }

    public interface _5
    {
        CompoundExpression Derived { get; }
        IReadOnlyList<_5> Support { get; }
        IBinding Binding { get; }
        CompoundExpression RuleDerived { get; }
        IReadOnlyList<CompoundExpression> RuleSupport { get; }
    }

    public interface _6
    {
        CompoundExpression Expression { get; }
        IReadOnlyList<_7> Supports { get; }
        IReadOnlyList<_7> Opposes { get; }
    }

    public interface _7
    {
        IReadOnlyList<CompoundExpression> RuleSupport { get; }
        CompoundExpression RuleDerived { get; }
        IBinding Binding { get; }
        IReadOnlyList<_6> Support { get; }
        _6 Derived { get; }
    }
}