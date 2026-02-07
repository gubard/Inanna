using Inanna.Ui;

namespace Inanna.Services;

public interface INonStatusBar;

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
        _viewModel.AddStatus(status);
    }

    public void RemoveStatus(object status)
    {
        _viewModel.RemoveStatus(status);
    }
}
