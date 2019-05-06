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
        bool IsReadOnly { get; }

        bool Add(CompoundExpression expression);
        bool Remove(CompoundExpression expression);
        bool Update(CompoundExpression remove, CompoundExpression add);
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

        bool Add(IEnumerable<CompoundExpression> expressions);
        bool Remove(IEnumerable<CompoundExpression> expressions);

        bool Update(IEnumerable<CompoundExpression> removals, IEnumerable<CompoundExpression> additions);
        bool Update(ICompoundExpressionDelta delta);
    }
}