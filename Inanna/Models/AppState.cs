using CommunityToolkit.Mvvm.ComponentModel;
using Inanna.Services;

namespace Inanna.Models;

public partial class AppState : ObservableObject
{
    public AppState(IStatusBarService statusBarService)
    {
        _statusBarService = statusBarService;
    }

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
        _serviceStatuses.Add(serviceState.ServiceName, new(serviceState));

        if (serviceState is not ObservableObject observable)
        {
            return;
        }

        observable.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(serviceState.Mode))
            {
                return;
            }

            foreach (var state in _serviceStates)
            {
                if (state.Value.Mode == ServiceMode.Offline)
                {
                    _statusBarService.AddStatus(_serviceStatuses[state.Key]);
                }
                else
                {
                    _statusBarService.RemoveStatus(_serviceStatuses[state.Key]);
                }
            }
        };
    }

    [ObservableProperty]
    private UserState? _user;

    private readonly Dictionary<string, IServiceState> _serviceStates = new();
    private readonly IStatusBarService _statusBarService;
    private readonly Dictionary<string, ServiceOfflineStatus> _serviceStatuses = new();
}

public class ServiceOfflineStatus
{
    public ServiceOfflineStatus(IServiceState state)
    {
        State = state;
    }

    public IServiceState State { get; }
}
