namespace Inanna.Models;

public class HomeMark
{
    public static readonly HomeMark Instance = new();
    public static readonly IEnumerable<object> IEnumerableInstance = [Instance];

    private HomeMark() { }
}
