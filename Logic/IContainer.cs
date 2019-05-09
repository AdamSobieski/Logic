namespace System.Collections.Generic
{
    public interface IContainer<in T>
    {
        bool Contains(T value);
    }
}