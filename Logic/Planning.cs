using Logic.Prolog.Collections;
using Logic.Prolog.Expressions;
using Logic.Prolog.Incremental;
using System.Collections.Generic;

namespace Logic.Planning
{
    public interface IAction
    {
        string Module { get; }
        string Name { get; }

        IReadOnlyList<VariableExpression> Parameters { get; }
        ICompoundExpressionList Preconditions { get; }
        ICompoundExpressionDelta Effects { get; }
    }
}