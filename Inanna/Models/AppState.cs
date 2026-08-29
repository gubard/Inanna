using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public sealed partial class AppState : ObservableObject
{
    public IEnumerable<IServiceState> ServiceStates => _serviceStates.Values;

    public ServiceMode GetServiceMode(string serviceName)
    {
        return _serviceStates[serviceName].Mode;
    }

    public void SetServiceMode(string serviceName, ServiceMode mode)
    {
        Dispatcher.UIThread.Invoke(
            () => _serviceStates[serviceName].Mode = mode,
            DispatcherPriority.Background
        );
    }

    public void ResetServiceModes()
    {
        Dispatcher.UIThread.Invoke(
            () =>
            {
                foreach (var serviceState in _serviceStates)
                {
                    serviceState.Value.Mode = ServiceMode.Online;
                }
            },
            DispatcherPriority.Background
        );
    }

    public void AddService(IServiceState serviceState)
    {
        _serviceStates.Add(serviceState.ServiceName, serviceState);
    }

    [ObservableProperty]
    private UserState? _user;

    private readonly Dictionary<string, IServiceState> _serviceStates = new();
}
