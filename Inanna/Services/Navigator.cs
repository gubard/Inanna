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
        return NavigateBackOrNullCore(ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable NavigateToAsync(object view, CancellationToken ct)
    {
        return NavigateToCore(view, ct).ConfigureAwait(false);
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
            refresh.RefreshAsync(CancellationToken.None);
        }
    }

    public void RefreshUiCurrentView()
    {
        if (CurrentView is IRefreshUi refresh)
        {
            refresh.RefreshUi();
        }
    }

    private async ValueTask<object?> NavigateBackOrNullCore(CancellationToken ct)
    {
        if (_stackViewModel.CurrentView is ISaveUi saveUi)
        {
            await saveUi.SaveUiAsync(ct);
        }

        _stackViewModel.PopView();

        if (_stackViewModel.CurrentView is IInitUi initUi)
        {
            await initUi.InitUiAsync(ct);
        }

        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);

        return _stackViewModel.CurrentView;
    }

    private async ValueTask NavigateToCore(object view, CancellationToken ct)
    {
        if (_stackViewModel.CurrentView is INonNavigate)
        {
            _stackViewModel.RemoveLastView();
        }

        if (_stackViewModel.CurrentView is ISaveUi saveUi)
        {
            await saveUi.SaveUiAsync(ct);
        }

        _stackViewModel.PushView(view);

        if (_stackViewModel.CurrentView is IInitUi initUi)
        {
            await initUi.InitUiAsync(ct);
        }

        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);
    }
}
