namespace Inanna.Models;

public sealed class ChangeOrderParameters<T>
    where T : IOrderedItem
{
    public ChangeOrderParameters(T item, bool isAfter)
    {
        Item = item;
        IsAfter = isAfter;
    }

    public T Item { get; }
    public bool IsAfter { get; }
}
