using Gaia.Services;

namespace Inanna.Models;

public interface IServiceState : IHealthCheck
{
    string ServiceName { get; }
    ServiceMode Mode { get; set; }
}
