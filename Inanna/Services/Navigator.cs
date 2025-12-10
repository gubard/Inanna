using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Ui;

namespace Inanna.Services;

public interface INavigator
{
    public event ViewChangedEventHandler? ViewChanged;
    bool IsEmpty { get; }
    object? CurrentView { get; }
    ValueTask<object?> NavigateBackOrNullAsync(CancellationToken ct);
    ValueTask NavigateToAsync(object view, CancellationToken ct);
    ValueTask RefreshCurrentViewAsync(CancellationToken ct);
    void RefreshCurrentView();
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

    public ValueTask<object?> NavigateBackOrNullAsync(CancellationToken ct)
    {
        _stackViewModel.PopView();
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);

        return ValueTask.FromResult(_stackViewModel.CurrentView);
    }

    public ValueTask NavigateToAsync(object view, CancellationToken ct)
    {
        _stackViewModel.PushView(view);
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);

        return ValueTask.CompletedTask;
    }

    public ValueTask RefreshCurrentViewAsync(CancellationToken ct)
    {
        if (CurrentView is IRefresh refresh)
        {
            return refresh.RefreshAsync(ct);
        }
        
        return ValueTask.CompletedTask;
    }

    public void RefreshCurrentView()
    {
        if (CurrentView is IRefresh refresh)
        {
            refresh.Refresh();
        }
    }
}