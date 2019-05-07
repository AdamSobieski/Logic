using Logic.Expressions;
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
        bool Replace(CompoundExpression remove, CompoundExpression add);
    }

    public interface ICompoundExpressionList : ICompoundExpressionCollection, IEnumerable<CompoundExpression>
    {
        int Count { get; }

        CompoundExpression this[int index] { get; }
    }
}