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

    public interface _9
    {
        CompoundExpression Expression { get; }
        IReadOnlyList<_10> IncomingEdges { get; }
        IReadOnlyList<_10> OutgoingEdges { get; }
    }

    public interface _10
    {
        CompoundExpression Expression { get; } // Relation(Source.Expression, Target.Expression)

        _9 Source { get; }
        PredicateExpression Relation { get; }
        _9 Target { get; }

        IReadOnlyList<_11> Support { get; }
    }

    public interface _11
    {
        IReadOnlyList<CompoundExpression> RuleBody { get; }
        CompoundExpression RuleHead { get; }
        IBinding Binding { get; }
        IReadOnlyList<_9> Body { get; }
        _10 Head { get; } // Head.Expression = Head.Relation(Head.Source.Expression, Head.Target.Expression)
    }

    public interface _12
    {
        CompoundExpression Expression { get; }
        IReadOnlyList<_13> IncomingEdges { get; }
        IReadOnlyList<_13> OutgoingEdges { get; }
    }

    public interface _13 : _12
    {
        //CompoundExpression Expression { get; } // Relation(Source.Expression, Target.Expression)
        //IReadOnlyList<_13> IncomingEdges { get; }
        //IReadOnlyList<_13> OutgoingEdges { get; }

        _12 Source { get; }
        PredicateExpression Relation { get; }
        _12 Target { get; }

        IReadOnlyList<_14> Support { get; }
    }

    public interface _14
    {
        IReadOnlyList<CompoundExpression> RuleBody { get; }
        CompoundExpression RuleHead { get; }
        IBinding Binding { get; }
        IReadOnlyList<_12> Body { get; }
        _12 Head { get; }
    }
}