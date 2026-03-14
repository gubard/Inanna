using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed class ServiceOfflineStatusViewModel : ViewModelBase
{
    public ServiceOfflineStatusViewModel(
        ISafeExecuteWrapper safeExecuteWrapper,
        IServiceState state,
        InannaCommands inannaCommands
    )
        : base(safeExecuteWrapper)
    {
        State = state;
        InannaCommands = inannaCommands;
    }

    public IServiceState State { get; }
    public InannaCommands InannaCommands { get; }
}
