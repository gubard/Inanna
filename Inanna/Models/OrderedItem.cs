namespace Inanna.Models;

public interface IOrderedItem
{
    public Guid Id { get; }
    public uint OrderIndex { get; }
    public bool IsChangingOrder { get; set; }
}
