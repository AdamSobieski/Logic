/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Expressions;
using System.Collections.Generic;

namespace Logic.Argumentation
{
    // (1) proofs and justifications

    public interface _1
    {
        IReadOnlyList<CompoundExpression> RuleFrom { get; }
        IReadOnlyList<CompoundExpression> RuleDerive { get; }
        IBinding Binding { get; }
        IReadOnlyList<CompoundExpression> Support { get; }
        IReadOnlyList<CompoundExpression> Derived { get; }
    }

    public interface _2
    {
        IReadOnlyList<CompoundExpression> RuleFrom { get; }
        CompoundExpression RuleDerive { get; }
        IBinding Binding { get; }
        IReadOnlyList<CompoundExpression> Support { get; }
        CompoundExpression Derived { get; }
    }

    public interface _3
    {
        CompoundExpression Rule { get; }
        IBinding Binding { get; }
        CompoundExpression Expression { get; }
    }
}
