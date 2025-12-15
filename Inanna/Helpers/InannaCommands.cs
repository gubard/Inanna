using System.Windows.Input;
using Gaia.Helpers;
using Inanna.Services;

namespace Inanna.Helpers;

public static class InannaCommands
{
    static InannaCommands()
    {
        var navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        NavigateToCommand = UiHelper.CreateCommand<Type>(
            (type, ct) => navigator.NavigateToAsync(DiHelper.ServiceProvider.GetService(type), ct)
        );
    }

    public static readonly ICommand NavigateToCommand;
}
