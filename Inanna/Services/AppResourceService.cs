using Avalonia;

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
        if (!_app.TryGetResource(key, _app.ActualThemeVariant, out var value))
        {
            throw new NullReferenceException($"Resource {key} not found");
        }

        return (T)value!;
    }
}
