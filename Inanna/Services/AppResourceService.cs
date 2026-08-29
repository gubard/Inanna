using Avalonia;
using Avalonia.Threading;
using Gaia.Helpers;
using Inanna.Helpers;
using Microsoft.Extensions.Logging;

namespace Inanna.Services;

public interface IAppResourceService
{
    T GetResource<T>(string key);
}

public sealed class AppResourceService : IAppResourceService
{
    public AppResourceService(Application app, ILogger logger)
    {
        _app = app;
        _logger = logger;
    }

    public T GetResource<T>(string key)
    {
        var actualThemeVariant = Dispatcher.UIThread.Invoke(
            () => _app.ActualThemeVariant,
            DispatcherPriority.Background
        );

        if (_app.TryGetResource(key, actualThemeVariant, out var value))
        {
            var result = (T)value!;

            return result.ThrowIfNull();
        }

        _logger.NotResourceKey(key);
        throw new NullReferenceException($"Resource {key} not found");
    }

    private readonly ILogger _logger;
    private readonly Application _app;
}
