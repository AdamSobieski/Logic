/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

namespace System.Collections.Generic
{
    public interface IDelta<out T>
    {
        IReadOnlyList<T> Remove { get; }
        IReadOnlyList<T> Add { get; }
    }
}