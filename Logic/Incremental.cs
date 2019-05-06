using Logic.Collections;

namespace Logic.Incremental
{
    public interface ICompoundExpressionDelta
    {
        ICompoundExpressionList Retractions { get; }
        ICompoundExpressionList Assertions { get; }
    }
}