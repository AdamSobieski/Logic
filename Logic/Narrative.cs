/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Expressions;
using System.Collections.Generic;

namespace Logic.Narrative
{
    public interface IEvent
    {
        VariableExpression Parameter { get; }
        IReadOnlyList<CompoundExpression> Expressions { get; }
    }

    public interface IEventNode : IEvent
    {
        IReadOnlyList<IEventEdge> IncomingEdges { get; }
        IReadOnlyList<IEventEdge> OutgoingEdges { get; }
    }

    public interface IEventEdge
    {
        CompoundExpression Expression { get; }

        IEvent Source { get; }
        PredicateExpression Relation { get; }
        IEvent Target { get; }
    }

    public interface IReadOnlyEventGraph
    {
        IReadOnlyCollection<IEvent> Events { get; }
        IReadOnlyCollection<IEventEdge> Relations { get; }

        //IReadOnlyCollection<IReadOnlyEventGraph> Subgraphs { get; }
        //IReadOnlyCollection<IReadOnlyEventGraph> Supergraphs { get; }

        VariableExpression Parameter { get; }
        IReadOnlyList<CompoundExpression> EmergentSemantics { get; }
    }
}