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
    public interface IArgument : IIndividual
    {
        CompoundExpression Expression { get; }
    }

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

    public interface IArgumentDerivation : IIndividual
    {
        IReadOnlyList<CompoundExpression> RuleBody { get; }
        CompoundExpression RuleHead { get; }
        IReadOnlyVariableBindings Bindings { get; }
        IReadOnlyList<IArgument> Body { get; }
        IArgumentDerived Head { get; }
    }

    public interface IArgumentGraph : IIndividual
    {
        IQueryable<IArgument> Nodes { get; }
        IQueryable<IArgumentEdge> Edges { get; }
    }
}