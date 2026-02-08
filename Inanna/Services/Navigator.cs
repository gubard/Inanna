using System.Runtime.CompilerServices;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Helpers;
using Inanna.Ui;

namespace Inanna.Services;

public interface INonNavigate;

public interface INavigator
{
    event ViewChangedEventHandler? ViewChanged;

    bool IsEmpty { get; }
    object? CurrentView { get; }

    ConfiguredValueTaskAwaitable<object?> NavigateBackOrNullAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable NavigateToAsync(object view, CancellationToken ct);
    ConfiguredValueTaskAwaitable RefreshCurrentViewAsync(CancellationToken ct);
}

public delegate void ViewChangedEventHandler(object? sender, object? view);

public sealed class Navigator : ObservableObject, INavigator
{
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

    public event ViewChangedEventHandler? ViewChanged;

    public bool IsEmpty => _stackViewModel.IsEmpty;
    public object? CurrentView => _stackViewModel.CurrentView;

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

    private readonly StackViewModel _stackViewModel;

    private async ValueTask<object?> NavigateBackOrNullCore(CancellationToken ct)
    {
        if (_stackViewModel.CurrentView is ISaveUi saveUi)
        {
            await saveUi.SaveUiAsync(ct);
        }

        Dispatcher.UIThread.Invoke(() => _stackViewModel.RemoveLastView());

        if (_stackViewModel.GetCurrentView() is IInitUi initUi)
        {
            await initUi.InitUiAsync(ct);
        }

        Dispatcher.UIThread.Invoke(() => _stackViewModel.SetCurrentView());
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);

        if (_stackViewModel.CurrentView is ILoadUi loadUi)
        {
            await loadUi.LoadUiAsync(ct);
        }

        return _stackViewModel.CurrentView;
    }

    private async ValueTask NavigateToCore(object view, CancellationToken ct)
    {
        if (_stackViewModel.CurrentView is ISaveUi saveUi)
        {
            await saveUi.SaveUiAsync(ct);
        }

        if (_stackViewModel.CurrentView is INonNavigate)
        {
            Dispatcher.UIThread.Invoke(() => _stackViewModel.RemoveLastView());
        }

        if (view is IInitUi initUi)
        {
            await initUi.InitUiAsync(ct);
        }

        Dispatcher.UIThread.Invoke(() => _stackViewModel.PushView(view));
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);

        if (view is ILoadUi loadUi)
        {
            await loadUi.LoadUiAsync(ct);
        }
    }
}
