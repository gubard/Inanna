using Avalonia.Controls;

namespace Inanna.Helpers;

public static class StringExtension
{
    public static TextBlock ToDialogHeader(this string str)
    {
        return new() { Text = str, Classes = { "h3", "text-wrap" } };
    }
}
