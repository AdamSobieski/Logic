using Logic.Expressions;
using Logic.Incremental;
using System.Collections.Generic;

namespace Logic.Collections
{
    public interface ICompoundExpressionContainer
    {
        bool Contains(CompoundExpression expression);
    }

    public interface ICompoundExpressionCollection : ICompoundExpressionContainer
    {
        bool Add(CompoundExpression expression);
        bool Remove(CompoundExpression expression);
    }

    public interface ICompoundExpressionList : ICompoundExpressionCollection, IEnumerable<CompoundExpression>
    {
        int Count { get; }

        CompoundExpression this[int index] { get; }
    }

    public interface IKnowledgebase : ICompoundExpressionCollection
    {
        bool AddRule(CompoundExpression rule);
        bool RemoveRule(CompoundExpression rule);
        bool ContainsRule(CompoundExpression rule);

        bool AddAll(IEnumerable<CompoundExpression> expressions);
        bool RemoveAll(IEnumerable<CompoundExpression> expressions);

        bool ProcessDelta(ICompoundExpressionDelta delta);
    }
}