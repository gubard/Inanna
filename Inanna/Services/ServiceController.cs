using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Gaia.Services;

namespace Inanna.Services;

public interface IServiceController
{
    ConfiguredValueTaskAwaitable<IValidationErrors> RefreshServicesAsync(CancellationToken ct);
}

public sealed class ServiceController : IServiceController
{
    public ServiceController(IEnumerable<IUiService> uiServices)
    {
        _uiServices = uiServices;
    }

    public ConfiguredValueTaskAwaitable<IValidationErrors> RefreshServicesAsync(
        CancellationToken ct
    )
    {
        return RefreshServicesCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<IValidationErrors> RefreshServicesCore(CancellationToken ct)
    {
        var errors = await TaskHelper.WhenAllAsync(
            _uiServices.Select(s => s.RefreshServiceAsync(ct)).ToArray(),
            ct
        );

        return errors.Combine();
    }

    private readonly IEnumerable<IUiService> _uiServices;
}
