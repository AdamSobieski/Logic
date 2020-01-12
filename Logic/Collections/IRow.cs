namespace Logic.Collections
{
    public interface IRow
    {
        object this[int index] { get; }
        int Length { get; }
    }
}