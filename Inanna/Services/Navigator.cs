using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Ui;

namespace Inanna.Services;

public interface INavigator
{
    Task<object?> NavigateBackOrNullAsync(CancellationToken ct);
    Task NavigateToAsync(object view, CancellationToken ct);
    bool IsEmpty { get; }
    object? CurrentView { get; }
    public event ViewChangedEventHandler? ViewChanged;
}

public delegate void ViewChangedEventHandler(object? sender, object? view);

public class Navigator : ObservableObject, INavigator
{
    private readonly StackViewModel _stackViewModel;

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

    public Task<object?> NavigateBackOrNullAsync(CancellationToken ct)
    {
        _stackViewModel.PopView();
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);

        return Task.FromResult(_stackViewModel.CurrentView);
    }

    public Task NavigateToAsync(object view, CancellationToken ct)
    {
        _stackViewModel.PushView(view);
        ViewChanged?.Invoke(this, _stackViewModel.CurrentView);

        return Task.CompletedTask;
    }

    public bool IsEmpty => _stackViewModel.IsEmpty;
    public object? CurrentView => _stackViewModel.CurrentView;
    public event ViewChangedEventHandler? ViewChanged;
}