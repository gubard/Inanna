using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Inanna.Models;

namespace Inanna.Ui;

public sealed partial class StatusBarViewModel : ViewModelBase
{
    public StatusBarViewModel(ViewModelServices services)
        : base(services) { }

    public IEnumerable<object> Statuses => _statuses;

    public void AddStatus(object status)
    {
        if (_statuses.Contains(status))
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            _statuses.Add(status);
            IsVisible = true;
        });
    }

    public void RemoveStatus(object status)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _statuses.Remove(status);

            if (_statuses.Count == 0)
            {
                IsVisible = false;
            }
        });
    }

    [ObservableProperty]
    private bool _isVisible;

    private readonly AvaloniaList<object> _statuses = new();

    [RelayCommand]
    private void Hide()
    {
        Dispatcher.UIThread.Post(() => IsVisible = false);
    }
}
