using System.Runtime.CompilerServices;
using Avalonia.Threading;
using Inanna.Controls;
using Inanna.Models;
using Inanna.Ui;

namespace Inanna.Services;

public interface IDialogService
{
    DialogButton CancelButton { get; }
    DialogButton OkButton { get; }

    ConfiguredValueTaskAwaitable ShowMessageBoxAsync(DialogViewModel dialog, CancellationToken ct);
    ConfiguredValueTaskAwaitable CloseMessageBoxAsync(CancellationToken ct);
}

public sealed class DialogService : IDialogService
{
    public DialogService(
        string dialogId,
        IAppResourceService appResourceService,
        ICommandFactory commandFactory,
        IInannaViewModelFactory factory
    )
    {
        _dialogId = dialogId;
        _stackViewModel = factory.CreateStack();
        _taskStack = new();

        CancelButton = new(
            appResourceService.GetResource<string>("Lang.Cancel"),
            commandFactory.CreateCommand(async ct => await CloseMessageBoxAsync(ct)),
            null,
            DialogButtonType.Normal
        );

        OkButton = new(
            appResourceService.GetResource<string>("Lang.Ok"),
            commandFactory.CreateCommand(async ct => await CloseMessageBoxAsync(ct)),
            null,
            DialogButtonType.Primary
        );
    }

    public DialogButton CancelButton { get; }
    public DialogButton OkButton { get; }

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
            Dispatcher.UIThread.Invoke(() => DialogControl.ShowDialog(_dialogId, _stackViewModel));
        }

        if (_stackViewModel.CurrentView is ISave saveUi)
        {
            await saveUi.SaveAsync(ct);
        }

        Dispatcher.UIThread.Invoke(() => _stackViewModel.PushViewUi(dialog));
        var taskCompletionSource = new TaskCompletionSource();
        _taskStack.Push(taskCompletionSource);
        ct.ThrowIfCancellationRequested();
        await taskCompletionSource.Task;
    }

    private async ValueTask CloseMessageBoxCore(CancellationToken ct)
    {
        if (_stackViewModel.CurrentView is ISave saveUi)
        {
            await saveUi.SaveAsync(ct);
        }

        Dispatcher.UIThread.Invoke(() => _stackViewModel.RemoveLastViewUi());
        Dispatcher.UIThread.Invoke(() => _stackViewModel.SetCurrentViewUi());

        if (_stackViewModel.CurrentView is null)
        {
            Dispatcher.UIThread.Invoke(() => DialogControl.CloseDialog(_dialogId));
        }

        if (_taskStack.Count != 0)
        {
            _taskStack.Pop().SetResult();
        }
    }
}
