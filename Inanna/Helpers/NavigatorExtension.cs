using System.Runtime.CompilerServices;
using Inanna.Services;

namespace Inanna.Helpers;

public static class NavigatorExtension
{
    public static ConfiguredValueTaskAwaitable NavigateToAsync<TView>(
        this INavigator navigator,
        Gaia.Services.IServiceProvider serviceProvider,
        CancellationToken ct
    )
        where TView : notnull
    {
        return navigator.NavigateToAsync(serviceProvider.GetService<TView>(), ct);
    }
}
