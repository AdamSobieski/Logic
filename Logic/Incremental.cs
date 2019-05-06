using Logic.Collections;

namespace Logic.Incremental
{
    public interface ICompoundExpressionDelta
    {
        ICompoundExpressionList Removals { get; }
        ICompoundExpressionList Additions { get; }
    }
}