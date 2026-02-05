using Avalonia.Threading;
using Inanna.Ui;

namespace Inanna.Services;

public interface IStatusBarService
{
    void AddStatus(object status);
    void RemoveStatus(object status);
}

public sealed class StatusBarService : IStatusBarService
{
    private readonly StatusBarViewModel _viewModel;

    public StatusBarService(StatusBarViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public void AddStatus(object status)
    {
        if (_viewModel.Statuses.Contains(status))
        {
            return;
        }

        Dispatcher.UIThread.Post(() => _viewModel.Statuses.Add(status));
    }

    public void RemoveStatus(object status)
    {
        Dispatcher.UIThread.Post(() => _viewModel.Statuses.Remove(status));
    }
}
