/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Expressions;
using System.Collections.Generic;

// to do: (1) model for the processing of multiple interpretations

namespace System.Collections.Specialized
{
    public interface ISemanticTree
    {
        ISemanticTree Parent { get; }
        IList<ISemanticTree> Children { get; }

        IList<CompoundExpression> Semantics { get; }
    }
}