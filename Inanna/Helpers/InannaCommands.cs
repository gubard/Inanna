using System.Windows.Input;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Services;

namespace Inanna.Helpers;

public static class InannaCommands
{
    static InannaCommands()
    {
        var navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        var openerLink = DiHelper.ServiceProvider.GetService<IOpenerLink>();

        NavigateToCommand = UiHelper.CreateCommand<Type>(
            (type, ct) => navigator.NavigateToAsync(DiHelper.ServiceProvider.GetService(type), ct)
        );

        OpenLinkCommand = UiHelper.CreateCommand<Uri>(
            (uri, ct) => openerLink.OpenLinkAsync(uri, ct)
        );
    }

    public static readonly ICommand NavigateToCommand;
    public static readonly ICommand OpenLinkCommand;
}
