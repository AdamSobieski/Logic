using Logic.Prolog.Expressions;
using Logic.Prolog.Incremental;
using System.Collections.Generic;

namespace Logic.Prolog.Collections
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
        bool Replace(CompoundExpression remove, CompoundExpression add);

        bool Add(IEnumerable<CompoundExpression> expressions);
        bool Remove(IEnumerable<CompoundExpression> expressions);
        bool Replace(IEnumerable<CompoundExpression> removals, IEnumerable<CompoundExpression> additions);

        bool Replace(ICompoundExpressionDelta delta);
    }

    public interface ICompoundExpressionList : ICompoundExpressionCollection, IEnumerable<CompoundExpression>
    {
        int Count { get; }

        CompoundExpression this[int index] { get; }
    }
}