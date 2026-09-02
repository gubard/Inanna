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

    ConfiguredValueTaskAwaitable CloseMessageBoxAsync(CancellationToken ct);
    DialogButton CreateButton(
        object content,
        Func<CancellationToken, ValueTask> func,
        DialogButtonType type
    );

    ConfiguredValueTaskAwaitable ShowMessageBoxAsync(
        object header,
        object content,
        CancellationToken ct,
        params DialogButton[] buttons
    );
}

public sealed class DialogService : IDialogService
{
    public DialogService(
        string dialogId,
        ICommandFactory commandFactory,
        IInannaViewModelFactory factory,
        ViewModelServices services
    )
    {
        _dialogId = dialogId;
        _commandFactory = commandFactory;
        _services = services;
        _stackViewModel = factory.CreateStack();
        _taskStack = new();

        CancelButton = new(
            _services.AppResourceService.GetResource<string>("Lang.Cancel"),
            commandFactory.CreateCommand(async ct => await CloseMessageBoxAsync(ct)),
            null,
            DialogButtonType.Normal
        );

        OkButton = new(
            _services.AppResourceService.GetResource<string>("Lang.Ok"),
            commandFactory.CreateCommand(async ct => await CloseMessageBoxAsync(ct)),
            null,
            DialogButtonType.Primary
        );
    }

    public DialogButton CancelButton { get; }
    public DialogButton OkButton { get; }

    public DialogButton CreateButton(
        object content,
        Func<CancellationToken, ValueTask> func,
        DialogButtonType type
    )
    {
        return new(content, _commandFactory.CreateCommand(func), null, type);
    }

    public ConfiguredValueTaskAwaitable ShowMessageBoxAsync(
        object header,
        object content,
        CancellationToken ct,
        params DialogButton[] buttons
    )
    {
        return ShowMessageBoxCore(header, content, ct, buttons).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable CloseMessageBoxAsync(CancellationToken ct)
    {
        return CloseMessageBoxCore(ct).ConfigureAwait(false);
    }

    private readonly ICommandFactory _commandFactory;
    private readonly ViewModelServices _services;
    private readonly Stack<TaskCompletionSource> _taskStack;
    private readonly StackViewModel _stackViewModel;
    private readonly string _dialogId;

    private async ValueTask ShowMessageBoxCore(
        object header,
        object content,
        CancellationToken ct,
        params DialogButton[] buttons
    )
    {
        if (
            !Dispatcher.UIThread.Invoke(
                () => DialogControl.IsShowDialog(_dialogId),
                DispatcherPriority.Background
            )
        )
        {
            Dispatcher.UIThread.Invoke(
                () => DialogControl.ShowDialog(_dialogId, _stackViewModel),
                DispatcherPriority.Background,
                ct
            );
        }

        if (_stackViewModel.CurrentView is ISave saveUi)
        {
            await saveUi.SaveAsync(ct);
        }

        Dispatcher.UIThread.Invoke(
            () =>
                _stackViewModel.PushViewUi(
                    new DialogViewModel(header, content, _services, buttons)
                ),
            DispatcherPriority.Background,
            ct
        );

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

        Dispatcher.UIThread.Invoke(
            () => _stackViewModel.RemoveLastViewUi(),
            DispatcherPriority.Background,
            ct
        );
        Dispatcher.UIThread.Invoke(
            () => _stackViewModel.SetCurrentViewUi(),
            DispatcherPriority.Background,
            ct
        );

        if (_stackViewModel.CurrentView is null)
        {
            Dispatcher.UIThread.Invoke(
                () => DialogControl.CloseDialog(_dialogId),
                DispatcherPriority.Background,
                ct
            );
        }

        if (_taskStack.Count != 0)
        {
            _taskStack.Pop().SetResult();
        }
    }
}
