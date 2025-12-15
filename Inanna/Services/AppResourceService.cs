using Avalonia;

namespace Inanna.Services;

public interface IAppResourceService
{
    T GetResource<T>(string key);
}

public class AppResourceService : IAppResourceService
{
    private readonly Application _application;

    public AppResourceService(Application application)
    {
        _application = application;
    }

    public T GetResource<T>(string key)
    {
        if (!_application.TryGetResource(key, null, out var value))
        {
            throw new NullReferenceException($"Resource {key} not found");
        }

        return (T)value!;
    }
}
