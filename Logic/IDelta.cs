/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using System.Collections.Generic;

namespace Logic.Incremental
{
    public interface IDelta<out T>
    {
        IReadOnlyList<T> Remove { get; }
        IReadOnlyList<T> Add { get; }
    }
}