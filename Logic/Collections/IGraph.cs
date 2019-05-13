/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

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
        IReadOnlyCollection<TNode> Nodes { get; }
        IReadOnlyCollection<IEdge<TNode, TEdgeValue>> Edges { get; }
    }

    // public interface ISemanticGraphEdge<out TNode> : IEdge<TNode, Logic.Expressions.PredicateExpression> { }
}