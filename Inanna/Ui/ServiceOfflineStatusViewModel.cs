using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed class ServiceOfflineStatusViewModel : ViewModelBase
{
    public ServiceOfflineStatusViewModel(
        ViewModelServices services,
        IServiceState state,
        InannaCommands inannaCommands
    )
        : base(services)
    {
        State = state;
        InannaCommands = inannaCommands;
    }

    public IServiceState State { get; }
    public InannaCommands InannaCommands { get; }
}
