using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed partial class NavigationBarViewModel : ViewModelBase
{
    public NavigationBarViewModel(
        INavigator navigator,
        ViewModelServices services,
        InannaCommands commands,
        object defaultHeader
    )
        : base(services)
    {
        _navigator = navigator;
        Commands = commands;
        Header = defaultHeader;

        _navigator.ViewChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(IsCanBack));
            OnPropertyChanged(nameof(Header));
            OnPropertyChanged(nameof(IsHeaderVisible));
        };
    }

    public bool IsCanBack => !_navigator.IsEmpty;
    public bool IsHeaderVisible => _navigator.CurrentView is not INonHeader;
    public InannaCommands Commands { get; }

    public object Header =>
        _navigator.CurrentView switch
        {
            IHeader header => header.Header,
            _ => field,
        };

    private readonly INavigator _navigator;

    [ObservableProperty]
    private bool _showPane;

    [RelayCommand]
    private async Task BackAsync(CancellationToken ct)
    {
        await WrapCommandAsync(() => BackCore(ct).ConfigureAwait(false), ct);
    }

    private async ValueTask BackCore(CancellationToken ct)
    {
        await _navigator.NavigateBackAsync(ct);
        OnPropertyChanged(nameof(IsCanBack));
    }
}
