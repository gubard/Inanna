using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed partial class NavigationBarViewModel : ViewModelBase
{
    public NavigationBarViewModel(
        INavigator navigator,
        IAppResourceService appResourceService,
        ViewModelServices services,
        InannaCommands сommands
    )
        : base(services)
    {
        _navigator = navigator;
        _appResourceService = appResourceService;
        Commands = сommands;

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
            _ => new TextBlock
            {
                Text = _appResourceService.GetResource<string>("Lang.AppName"),
                Classes = { "align-left-center", "h3" },
            },
        };

    private readonly INavigator _navigator;
    private readonly IAppResourceService _appResourceService;

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
