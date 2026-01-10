namespace Inanna.Models;

public class AppState
{
    public UserState? User { get; set; }

    public ServiceMode GetServiceMode(string serviceName)
    {
        return _serviceModes.GetValueOrDefault(serviceName, ServiceMode.Online);
    }

    public void SetServiceMode(string serviceName, ServiceMode mode)
    {
        _serviceModes[serviceName] = mode;
    }

    private readonly Dictionary<string, ServiceMode> _serviceModes = new();
}
