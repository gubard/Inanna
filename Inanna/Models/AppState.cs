using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public partial class AppState : ObservableObject
{
    public IEnumerable<IServiceState> ServiceStates => _serviceStates.Values;

    public ServiceMode GetServiceMode(string serviceName)
    {
        return _serviceStates[serviceName].Mode;
    }

    public void SetServiceMode(string serviceName, ServiceMode mode)
    {
        _serviceStates[serviceName].Mode = mode;
    }

    public void ResetServiceModes()
    {
        foreach (var serviceState in _serviceStates)
        {
            serviceState.Value.Mode = ServiceMode.Online;
        }
    }

    public void AddService(IServiceState serviceState)
    {
        _serviceStates.Add(serviceState.ServiceName, serviceState);
    }

    [ObservableProperty]
    private UserState? _user;

    private readonly Dictionary<string, IServiceState> _serviceStates = new();
}
