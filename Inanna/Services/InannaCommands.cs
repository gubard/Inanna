using System.Windows.Input;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;

namespace Inanna.Services;

public class InannaCommands
{
    public InannaCommands(IOpenerLink openerLink, INavigator navigator)
    {
        _openerLink = openerLink;
        _navigator = navigator;

        NavigateToCommand = UiHelper.CreateCommand<Type>(
            (type, ct) => navigator.NavigateToAsync(DiHelper.ServiceProvider.GetService(type), ct)
        );

        OpenLinkCommand = UiHelper.CreateCommand<Uri>(openerLink.OpenLinkAsync);
    }

    public ICommand NavigateToCommand { get; }
    public ICommand OpenLinkCommand { get; }

    private readonly INavigator _navigator;
    private readonly IOpenerLink _openerLink;
}
