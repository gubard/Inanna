using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public partial class AppState : ObservableObject
{
    [ObservableProperty]
    private UserState? _user;

    public ServiceMode GetServiceMode(string serviceName)
    {
        return _serviceModes.GetValueOrDefault(serviceName, ServiceMode.Online);
    }

    public void SetServiceMode(string serviceName, ServiceMode mode)
    {
        _serviceModes[serviceName] = mode;
    }

    public void ResetServiceModes()
    {
        _serviceModes.Clear();
    }

    private readonly Dictionary<string, ServiceMode> _serviceModes = new();
}
