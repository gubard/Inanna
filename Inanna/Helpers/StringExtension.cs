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

    public static TextBlock ToNotification(this string str)
    {
        return new() { Text = str, Classes = { "align-center", "h2", "m-5", "text-wrap" } };
    }

    public static TextBlock DispatchToNotification(this string str)
    {
        return Dispatcher.UIThread.Invoke(str.ToNotification);
    }
}
