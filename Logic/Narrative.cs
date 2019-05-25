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
    // see also: 
    // Cardona-Rivera, Rogelio E., and R. Michael Young. "Desiderata for a Computational Model of Human Online Narrative Sensemaking." (2019).
    //
    // to do: (1) graph structure, semantics and emergent semantics through automated reasoning processes
    //        (2) hermeneutics, interpretive inferencing and interpretations of: events, sequences of events, and narrative structure
    //            (a) an intelligent agent, utilizing a knowledgebase, can make inferences which include interpretive inferences
    //        (3) the modelling of story characters can be a use case scenario for the intelligent agents architecture

    public interface IEvent : IIndividual
    {
        IDelta<CompoundExpression> Effects { get; }

        IList<CompoundExpression> Semantics { get; }
    }

    public interface IEventEdge : IIndividual
    {
        CompoundExpression Expression { get; }

        IEvent Source { get; }
        PredicateExpression Relation { get; }
        IEvent Target { get; }
    }

    public interface IEventGraph : IIndividual
    {
        //string Id { get; set; }
        //IEventGraph GetSubgraphById(string id);

        IEventGraph Root { get; }

        IQueryable<IEvent> Nodes { get; }
        IQueryable<IEventEdge> Edges { get; }
        IQueryable<IEventGraph> Subgraphs { get; }

        IEvent FindOrCreateNode(/*...*/);
        IEventEdge FindOrCreateEdge(IEvent source, PredicateExpression relation, IEvent target);
        IEventGraph FindOrCreateSubgraph(IEnumerable<IEventEdge> edges);

        bool Add(IEventEdge edge);
        bool Remove(IEventEdge edge);
        bool Replace(IEventEdge remove, IEventEdge add);
        bool Contains(IEventEdge edge);

        IList<CompoundExpression> Semantics { get; }
    }
}