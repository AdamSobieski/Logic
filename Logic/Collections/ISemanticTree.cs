/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Expressions;
using System.Collections.Generic;

namespace System.Collections.Specialized
{
    public interface ISemanticTree
    {
        IList<ISemanticTree> Children { get; }

        IList<CompoundExpression> Semantics { get; }
    }
}
