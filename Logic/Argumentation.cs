/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Expressions;
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
        IReadOnlyList<CompoundExpression> RuleBody { get; }
        IReadOnlyList<CompoundExpression> RuleHead { get; }
        IBinding Binding { get; }
        IReadOnlyList<CompoundExpression> Body { get; }
        IReadOnlyList<CompoundExpression> Head { get; }
    }

    public interface _3
    {
        IReadOnlyList<CompoundExpression> RuleBody { get; }
        CompoundExpression RuleHead { get; }
        IBinding Binding { get; }
        IReadOnlyList<CompoundExpression> Body { get; }
        CompoundExpression Head { get; }
    }

    public interface _4
    {
        IReadOnlyList<CompoundExpression> RuleBody { get; }
        CompoundExpression RuleHead { get; }
        IBinding Binding { get; }
        IReadOnlyList<_4> Body { get; }
        CompoundExpression Head { get; }
    }

    public interface _5
    {
        CompoundExpression Expression { get; }
        IReadOnlyList<_6> SupportedBy { get; }
        IReadOnlyList<_6> OpposedBy { get; }
    }

    public interface _6
    {
        IReadOnlyList<CompoundExpression> RuleBody { get; }
        CompoundExpression RuleHead { get; }
        IBinding Binding { get; }
        IReadOnlyList<_5> Body { get; }
        _5 Head { get; }
    }

    public interface _7
    {
        CompoundExpression Expression { get; }
        IReadOnlyList<_8> SupportedBy { get; }
        IReadOnlyList<_8> OpposedBy { get; }
        IReadOnlyList<_8> Supports { get; }
        IReadOnlyList<_8> Opposes { get; }
    }

    public interface _8
    {
        IReadOnlyList<CompoundExpression> RuleBody { get; }
        CompoundExpression RuleHead { get; }
        IBinding Binding { get; }
        IReadOnlyList<_7> Body { get; }
        _7 Head { get; }
    }
}