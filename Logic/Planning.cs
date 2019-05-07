using Logic.Collections;
using Logic.Expressions;
using Logic.Incremental;
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