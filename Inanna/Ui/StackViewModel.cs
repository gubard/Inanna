using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Models;

namespace Inanna.Ui;

public partial class StackViewModel : ViewModelBase
{
    private readonly Stack<object> _views = new();
    [ObservableProperty] private object? _currentView;
    public bool IsEmpty => _views.Count <= 1;

    public void PushView(object view)
    {
        CurrentView = view;
        _views.Push(view);
        OnPropertyChanged(nameof(IsEmpty));
    }

    public void PopView()
    {
        if (_views.Count == 0)
        {
            CurrentView = null;
            OnPropertyChanged(nameof(IsEmpty));

            return;
        }

        _views.Pop();

        if (_views.Count == 0)
        {
            CurrentView = null;
            OnPropertyChanged(nameof(IsEmpty));

            return;
        }

        CurrentView = _views.Peek();
        OnPropertyChanged(nameof(IsEmpty));
    }

    public void RemoveLastView()
    {
        if (_views.Count == 0)
        {
            return;
        }

        _views.Pop();
        OnPropertyChanged(nameof(IsEmpty));
    }
}