using System.Runtime.CompilerServices;
using Avalonia.Threading;
using Inanna.Controls;
using Inanna.Ui;

namespace Inanna.Services;

public interface IDialogService
{
    ConfiguredValueTaskAwaitable ShowMessageBoxAsync(DialogViewModel dialog, CancellationToken ct);
    void CloseMessageBox();
}

public class DialogService : IDialogService
{
    private readonly Stack<TaskCompletionSource> _taskStack;
    private readonly StackViewModel _stackViewModel;
    private readonly string _dialogId;

    public DialogService(string dialogId)
    {
        _dialogId = dialogId;
        _stackViewModel = new();
        _taskStack = new();
    }

    public ConfiguredValueTaskAwaitable ShowMessageBoxAsync(
        DialogViewModel dialog,
        CancellationToken ct
    )
    {
        return ShowMessageBoxCore(dialog, ct).ConfigureAwait(false);
    }

    private async ValueTask ShowMessageBoxCore(DialogViewModel dialog, CancellationToken ct)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _stackViewModel.PushView(dialog);
            DialogControl.ShowDialog(_dialogId, _stackViewModel);
        });

        var taskCompletionSource = new TaskCompletionSource();
        _taskStack.Push(taskCompletionSource);
        ct.ThrowIfCancellationRequested();
        await taskCompletionSource.Task;
    }

    public void CloseMessageBox()
    {
        _stackViewModel.PopView();

        if (_stackViewModel.CurrentView is null)
        {
            DialogControl.CloseDialog(_dialogId);
        }

        if (_taskStack.Count != 0)
        {
            _taskStack.Pop().SetResult();
        }
    }
}
