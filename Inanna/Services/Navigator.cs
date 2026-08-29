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

    ConfiguredValueTaskAwaitable NavigateBackAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable NavigateToAsync(object view, CancellationToken ct);
    ConfiguredValueTaskAwaitable RefreshCurrentViewAsync(CancellationToken ct);
    void RefreshCurrentViewUi();
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

    public ConfiguredValueTaskAwaitable NavigateBackAsync(CancellationToken ct)
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

    public void RefreshCurrentViewUi()
    {
        if (CurrentView is IRefreshUi refreshUi)
        {
            refreshUi.RefreshUi();
        }
    }

    private readonly StackViewModel _stackViewModel;

    private async ValueTask NavigateBackOrNullCore(CancellationToken ct)
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
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);
    }

    private async ValueTask NavigateToCore(object view, CancellationToken ct)
    {
        if (_stackViewModel.CurrentView is ISave saveUi)
        {
            await saveUi.SaveAsync(ct);
        }

        if (_stackViewModel.CurrentView is INonNavigate)
        {
            Dispatcher.UIThread.Invoke(
                () => _stackViewModel.RemoveLastViewUi(),
                DispatcherPriority.Background,
                ct
            );
        }

        Dispatcher.UIThread.Invoke(
            () => _stackViewModel.PushViewUi(view),
            DispatcherPriority.Background,
            ct
        );
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);
    }
}
