using Avalonia.Controls;
using Avalonia.Threading;

namespace Inanna.Helpers;

public static class StringExtension
{
    public static TextBlock ToDialogHeader(this string str)
    {
        return new() { Text = str, Classes = { "h3", "text-wrap" } };
    }

    public static TextBlock DispatchToDialogHeader(this string str)
    {
        return Dispatcher.UIThread.Invoke(str.ToDialogHeader);
    }
}
