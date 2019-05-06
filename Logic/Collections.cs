using Logic.Expressions;

namespace Logic.Collections
{
    public interface ICompoundExpressionCollection
    {
        bool Add(CompoundExpression value);
        bool Remove(CompoundExpression value);
        bool Contains(CompoundExpression value);
    }

    public interface ICompoundExpressionList : ICompoundExpressionCollection
    {
        int Count { get; }

        CompoundExpression this[int index] { get; }
    }
}