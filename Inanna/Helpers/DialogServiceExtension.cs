using Avalonia.Threading;
using Inanna.Services;

namespace Inanna.Helpers;

public static class DialogServiceExtension
{
    public static void DispatchCloseMessageBox(this IDialogService dialogService)
    {
        Dispatcher.UIThread.Post(dialogService.CloseMessageBox);
    }
}
