/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Expressions;
using System.Collections.Generic;

namespace Logic.Narrative
{
    // to do: (1) semantics and emergent semantics through derivation
    //        (2) hermeneutics, interpretive inferencing and interpretations of: events, sequences of events, and narrative structure

    public interface IEvent
    {
        VariableExpression Parameter { get; }
        IDelta<CompoundExpression> Expressions { get; } // IReadOnlyCollection<> or IDelta<> ?
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
        IReadOnlyCollection<IEvent> Nodes { get; }
        IReadOnlyCollection<IEventEdge> Edges { get; }

        //IReadOnlyCollection<IReadOnlyEventGraph> Subgraphs { get; }
        //IReadOnlyCollection<IReadOnlyEventGraph> Supergraphs { get; }

        VariableExpression Parameter { get; }
        IReadOnlyList<CompoundExpression> EmergentSemantics { get; }
    }
}