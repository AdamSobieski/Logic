/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using System.Linq;

namespace System.Collections.Generic
{
    public interface IEdge<out TNode, out TValue>
    {
        TNode Source { get; }
        TValue Value { get; }
        TNode Target { get; }
    }

    public interface IReadOnlyGraph<out TNode, out TEdgeValue>
    {
        IQueryable<TNode> Nodes { get; }
        IQueryable<IEdge<TNode, TEdgeValue>> Edges { get; }
    }

    // public interface ISemanticGraphEdge : IEdge<Logic.Expressions.Expression, Logic.Expressions.PredicateExpression> { }
}