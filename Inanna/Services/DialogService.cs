using Inanna.Controls;
using Inanna.Ui;

namespace Inanna.Services;

public interface IDialogService
{
    Task ShowMessageBoxAsync(DialogViewModel dialog);
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

    public Task ShowMessageBoxAsync(DialogViewModel dialog)
    {
        _stackViewModel.PushView(dialog);
        DialogControl.ShowDialog(_dialogId, _stackViewModel);
        var taskCompletionSource = new TaskCompletionSource();
        _taskStack.Push(taskCompletionSource);

        return taskCompletionSource.Task;
    }

    public void CloseMessageBox()
    {
        _stackViewModel.PopView();

        if (_stackViewModel.CurrentView is null)
        {
            DialogControl.CloseDialog(_dialogId);
        }

        _taskStack.Pop().SetResult();
    }
}