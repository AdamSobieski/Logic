/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Narrative
{
    // to do: (1) semantics and emergent semantics through derivation
    //        (2) hermeneutics, interpretive inferencing and interpretations of: events, sequences of events, and narrative structure
    //            (a) an intelligent agent, utilizing a knowledgebase, can make inferences which include interpretive inferences
    //        (3) the modelling of story characters can be a use case scenario for the intelligent agents architecture
    //
    // Cardona-Rivera, Rogelio E., and R. Michael Young. "Desiderata for a Computational Model of Human Online Narrative Sensemaking." (2019).
    // https://en.wikipedia.org/wiki/Fabula_and_syuzhet

    public interface IEvent : IIndividual
    {
        IDelta<CompoundExpression> Effects { get; }

        IReadOnlyList<CompoundExpression> Semantics { get; }
    }

    public interface IEventEdge : IIndividual
    {
        CompoundExpression Expression { get; }

        IEvent Source { get; }
        PredicateExpression Relation { get; }
        IEvent Target { get; }
    }

    public interface IReadOnlyEventGraph : IIndividual
    {
        IQueryable<IEvent> Nodes { get; }
        IQueryable<IEventEdge> Edges { get; }

        //IQueryable<IReadOnlyEventGraph> Subgraphs { get; }
        //IQueryable<IReadOnlyEventGraph> Supergraphs { get; }

        IReadOnlyList<CompoundExpression> Semantics { get; }
    }
}