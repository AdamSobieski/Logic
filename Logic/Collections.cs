using Logic.Expressions;
using Logic.Incremental;
using System.Collections.Generic;

namespace Logic.Collections
{
    public interface ICompoundExpressionCollection
    {
        bool Add(CompoundExpression value);
        bool Remove(CompoundExpression value);
        bool Contains(CompoundExpression value);
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

        bool AddAll(IEnumerable<CompoundExpression> values);
        bool RemoveAll(IEnumerable<CompoundExpression> values);

        bool ProcessDelta(ICompoundExpressionDelta delta);
    }
}