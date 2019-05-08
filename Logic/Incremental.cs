using Logic.Prolog.Collections;

namespace Logic.Prolog.Incremental
{
    public interface ICompoundExpressionDelta
    {
        ICompoundExpressionList Removals { get; }
        ICompoundExpressionList Additions { get; }
    }
}