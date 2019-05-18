/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Argumentation
{
    //public interface _1
    //{
    //    CompoundExpression Rule { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    CompoundExpression Expression { get; }
    //}

    //public interface _2
    //{
    //    IReadOnlyList<CompoundExpression> RuleBody { get; }
    //    IReadOnlyList<CompoundExpression> RuleHead { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    IReadOnlyList<CompoundExpression> Body { get; }
    //    IReadOnlyList<CompoundExpression> Head { get; }
    //}

    //public interface _3
    //{
    //    IReadOnlyList<CompoundExpression> RuleBody { get; }
    //    CompoundExpression RuleHead { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    IReadOnlyList<CompoundExpression> Body { get; }
    //    CompoundExpression Head { get; }
    //}

    //public interface _4
    //{
    //    IReadOnlyList<CompoundExpression> RuleBody { get; }
    //    CompoundExpression RuleHead { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    IReadOnlyList<_4> Body { get; }
    //    CompoundExpression Head { get; }
    //}

    //public interface _5
    //{
    //    CompoundExpression Expression { get; }
    //    IReadOnlyList<_6> SupportedBy { get; }
    //    IReadOnlyList<_6> OpposedBy { get; }
    //}

    //public interface _6
    //{
    //    IReadOnlyList<CompoundExpression> RuleBody { get; }
    //    CompoundExpression RuleHead { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    IReadOnlyList<_5> Body { get; }
    //    _5 Head { get; }
    //}

    //public interface _7
    //{
    //    CompoundExpression Expression { get; }
    //    IReadOnlyList<_8> SupportedBy { get; }
    //    IReadOnlyList<_8> OpposedBy { get; }
    //    IReadOnlyList<_8> Supports { get; }
    //    IReadOnlyList<_8> Opposes { get; }
    //}

    //public interface _8
    //{
    //    IReadOnlyList<CompoundExpression> RuleBody { get; }
    //    CompoundExpression RuleHead { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    IReadOnlyList<_7> Body { get; }
    //    _7 Head { get; }
    //}

    //public interface _9
    //{
    //    CompoundExpression Expression { get; }
    //    IReadOnlyList<_10> IncomingEdges { get; }
    //    IReadOnlyList<_10> OutgoingEdges { get; }
    //}

    //public interface _10
    //{
    //    CompoundExpression Expression { get; }

    //    _9 Source { get; }
    //    PredicateExpression Relation { get; }
    //    _9 Target { get; }

    //    IReadOnlyList<_11> SupportedBy { get; }
    //}

    //public interface _11
    //{
    //    IReadOnlyList<CompoundExpression> RuleBody { get; }
    //    CompoundExpression RuleHead { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    IReadOnlyList<_9> Body { get; }
    //    _10 Head { get; }
    //}

    //public interface _12
    //{
    //    CompoundExpression Expression { get; }
    //    IReadOnlyList<_13> IncomingEdges { get; }
    //    IReadOnlyList<_13> OutgoingEdges { get; }

    //    IReadOnlyList<_14> SupportedBy { get; }
    //}

    //public interface _13 : _12
    //{
    //    _12 Source { get; }
    //    PredicateExpression Relation { get; }
    //    _12 Target { get; }
    //}

    //public interface _14
    //{
    //    IReadOnlyList<CompoundExpression> RuleBody { get; }
    //    CompoundExpression RuleHead { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    IReadOnlyList<_12> Body { get; }
    //    _12 Head { get; }
    //}

    //public interface _15
    //{
    //    CompoundExpression Expression { get; }
    //    IReadOnlyList<_16> IncomingEdges { get; }
    //    IReadOnlyList<_16> OutgoingEdges { get; }

    //    IReadOnlyList<_17> SupportedBy { get; }
    //    IReadOnlyList<_17> OpposedBy { get; }
    //}

    //public interface _16 : _15
    //{
    //    _15 Source { get; }
    //    PredicateExpression Relation { get; }
    //    _15 Target { get; }
    //}

    //public interface _17
    //{
    //    IReadOnlyList<CompoundExpression> RuleBody { get; }
    //    CompoundExpression RuleHead { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    IReadOnlyList<_15> Body { get; }
    //    _15 Head { get; }
    //}

    //public interface _18
    //{
    //    CompoundExpression Expression { get; }
    //    IReadOnlyList<_19> IncomingEdges { get; }
    //    IReadOnlyList<_19> OutgoingEdges { get; }
    //}

    //public interface _19 : _18
    //{
    //    _18 Source { get; }
    //    PredicateExpression Relation { get; }
    //    _18 Target { get; }
    //}

    //public interface _20 : _18
    //{
    //    IReadOnlyList<CompoundExpression> RuleBody { get; }
    //    CompoundExpression RuleHead { get; }
    //    IReadOnlyVariableBindings Bindings { get; }
    //    IReadOnlyList<_18> Body { get; }
    //    _18 Head { get; }
    //}

    public interface IArgument
    {
        CompoundExpression Expression { get; }
    }

    //public interface IArgumentNode : IArgument
    //{
    //    IReadOnlyList<IArgumentEdge> IncomingEdges { get; }
    //    IReadOnlyList<IArgumentEdge> OutgoingEdges { get; }
    //}

    public interface IArgumentEdge : IArgument
    {
        IArgument Source { get; }
        PredicateExpression Relation { get; }
        IArgument Target { get; }
    }

    public interface IArgumentDerived : IArgument
    {
        IReadOnlyList<IArgumentDerivation> SupportedBy { get; }
        IReadOnlyList<IArgumentDerivation> OpposedBy { get; }
    }

    public interface IArgumentDerivation
    {
        IReadOnlyList<CompoundExpression> RuleBody { get; }
        CompoundExpression RuleHead { get; }
        IReadOnlyVariableBindings Bindings { get; }
        IReadOnlyList<IArgument> Body { get; }
        IArgumentDerived Head { get; }
    }

    public interface IReadOnlyArgumentGraph
    {
        IReadOnlyCollection<IArgument> Nodes { get; }
        IQueryable<IArgumentEdge> Edges { get; }
    }
}