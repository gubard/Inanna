namespace Inanna.Models;

public sealed class HomeMark
{
    public static readonly HomeMark Instance = new();

    private HomeMark() { }
}

public sealed class AddMark
{
    public static readonly AddMark Instance = new();

    private AddMark() { }
}
