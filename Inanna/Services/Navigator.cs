using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Helpers;
using Inanna.Ui;

namespace Inanna.Services;

public interface INonNavigate;

public interface INavigator
{
    public event ViewChangedEventHandler? ViewChanged;
    bool IsEmpty { get; }
    object? CurrentView { get; }
    ConfiguredValueTaskAwaitable<object?> NavigateBackOrNullAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable NavigateToAsync(object view, CancellationToken ct);
    ConfiguredValueTaskAwaitable RefreshCurrentViewAsync(CancellationToken ct);
    void RefreshCurrentView();
    void RefreshUiCurrentView();
}

public delegate void ViewChangedEventHandler(object? sender, object? view);

public class Navigator : ObservableObject, INavigator
{
    private readonly StackViewModel _stackViewModel;

    public event ViewChangedEventHandler? ViewChanged;

    public bool IsEmpty => _stackViewModel.IsEmpty;

    public object? CurrentView => _stackViewModel.CurrentView;

    public Navigator(StackViewModel stackViewModel)
    {
        _stackViewModel = stackViewModel;

        _stackViewModel.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(StackViewModel.CurrentView):
                {
                    OnPropertyChanged(nameof(IsEmpty));

                    break;
                }
            }
        };
    }

    public ConfiguredValueTaskAwaitable<object?> NavigateBackOrNullAsync(CancellationToken ct)
    {
        _stackViewModel.PopView();
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);

        return TaskHelper.FromResult(_stackViewModel.CurrentView);
    }

    public ConfiguredValueTaskAwaitable NavigateToAsync(object view, CancellationToken ct)
    {
        if (_stackViewModel.CurrentView is INonNavigate)
        {
            _stackViewModel.RemoveLastView();
        }

        _stackViewModel.PushView(view);
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);

        return TaskHelper.ConfiguredCompletedTask;
    }

    public ConfiguredValueTaskAwaitable RefreshCurrentViewAsync(CancellationToken ct)
    {
        if (CurrentView is IRefresh refresh)
        {
            return refresh.RefreshAsync(ct);
        }

        return TaskHelper.ConfiguredCompletedTask;
    }

    public void RefreshCurrentView()
    {
        if (CurrentView is IRefresh refresh)
        {
            refresh.Refresh();
        }
    }

    public void RefreshUiCurrentView()
    {
        if (CurrentView is IRefreshUi refresh)
        {
            refresh.RefreshUi();
        }
    }
}
