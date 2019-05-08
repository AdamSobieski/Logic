using System.Collections.Generic;
using Logic.Prolog.Expressions;

namespace Logic.Prolog.Incremental
{
    public interface ICompoundExpressionDelta
    {
        IReadOnlyList<CompoundExpression> Removals { get; }
        IReadOnlyList<CompoundExpression> Additions { get; }
    }
}