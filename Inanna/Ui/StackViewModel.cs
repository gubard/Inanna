using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;

namespace Inanna.Ui;

public sealed partial class StackViewModel : ViewModelBase
{
    public bool IsEmpty => _views.Count <= 1;

    public void PushViewUi(object view)
    {
        CurrentView = view;
        _views.Push(view);
        OnPropertyChanged(nameof(IsEmpty));
    }

    public void RemoveLastViewUi()
    {
        if (_views.Count == 0)
        {
            return;
        }

        _views.Pop();
        OnPropertyChanged(nameof(IsEmpty));
    }

    public void SetCurrentViewUi()
    {
        if (_views.Count == 0)
        {
            CurrentView = null;

            return;
        }

        CurrentView = _views.Peek();
    }

    public object? GetCurrentView()
    {
        if (_views.Count == 0)
        {
            return null;
        }

        return _views.Peek();
    }

    [ObservableProperty]
    private object? _currentView;

    private readonly Stack<object> _views = new();
}
