using System.Runtime.CompilerServices;
using Avalonia.Threading;
using Inanna.Controls;
using Inanna.Ui;

namespace Inanna.Services;

public interface IDialogService
{
    ConfiguredValueTaskAwaitable ShowMessageBoxAsync(DialogViewModel dialog, CancellationToken ct);
    ConfiguredValueTaskAwaitable CloseMessageBoxAsync(CancellationToken ct);
}

public sealed class DialogService : IDialogService
{
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

    public ConfiguredValueTaskAwaitable CloseMessageBoxAsync(CancellationToken ct)
    {
        return CloseMessageBoxCore(ct).ConfigureAwait(false);
    }

    private readonly Stack<TaskCompletionSource> _taskStack;
    private readonly StackViewModel _stackViewModel;
    private readonly string _dialogId;

    private async ValueTask ShowMessageBoxCore(DialogViewModel dialog, CancellationToken ct)
    {
        if (!Dispatcher.UIThread.Invoke(() => DialogControl.IsShowDialog(_dialogId)))
        {
            Dispatcher.UIThread.Post(() => DialogControl.ShowDialog(_dialogId, _stackViewModel));
        }

        if (_stackViewModel.CurrentView is ISaveUi saveUi)
        {
            await saveUi.SaveUiAsync(ct);
        }

        await dialog.InitAsync(ct);
        Dispatcher.UIThread.Invoke(() => _stackViewModel.PushViewUi(dialog));
        await dialog.LoadUiAsync(ct);
        var taskCompletionSource = new TaskCompletionSource();
        _taskStack.Push(taskCompletionSource);
        ct.ThrowIfCancellationRequested();
        await taskCompletionSource.Task;
    }

    private async ValueTask CloseMessageBoxCore(CancellationToken ct)
    {
        if (_stackViewModel.CurrentView is ISaveUi saveUi)
        {
            await saveUi.SaveUiAsync(ct);
        }

        Dispatcher.UIThread.Invoke(() => _stackViewModel.RemoveLastViewUi());

        if (_stackViewModel.GetCurrentView() is IInit initUi)
        {
            await initUi.InitAsync(ct);
        }

        Dispatcher.UIThread.Invoke(() => _stackViewModel.SetCurrentViewUi());

        if (_stackViewModel.CurrentView is ILoadUi loadUi)
        {
            await loadUi.LoadUiAsync(ct);
        }

        if (_stackViewModel.CurrentView is null)
        {
            Dispatcher.UIThread.Post(() => DialogControl.CloseDialog(_dialogId));
        }

        if (_taskStack.Count != 0)
        {
            _taskStack.Pop().SetResult();
        }
    }
}
