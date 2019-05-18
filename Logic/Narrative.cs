﻿/*********************************************************
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
    //        (3) modelling story characters can be a use case scenario for intelligent agents architecture
    //
    // Cardona-Rivera, Rogelio E., and R. Michael Young. "Desiderata for a Computational Model of Human Online Narrative Sensemaking." (2019).

    public interface IEvent : IIndividual
    {
        IDelta<CompoundExpression> Effects { get; }
    }

    public interface IEventEdge
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

        //IReadOnlyCollection<IReadOnlyEventGraph> Subgraphs { get; }
        //IReadOnlyCollection<IReadOnlyEventGraph> Supergraphs { get; }

        IReadOnlyList<CompoundExpression> EmergentSemantics { get; }
    }
}