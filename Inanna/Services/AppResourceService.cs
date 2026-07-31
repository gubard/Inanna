using Avalonia;
using Avalonia.Threading;

namespace Inanna.Services;

public interface IAppResourceService
{
    T GetResource<T>(string key);
}

public sealed class AppResourceService : IAppResourceService
{
    private readonly Application _app;

    public AppResourceService(Application app)
    {
        _app = app;
    }

    public T GetResource<T>(string key)
    {
        var actualThemeVariant = Dispatcher.UIThread.Invoke(() => _app.ActualThemeVariant);

        if (!_app.TryGetResource(key, actualThemeVariant, out var value))
        {
            throw new NullReferenceException($"Resource {key} not found");
        }

        return (T)value!;
    }
}
