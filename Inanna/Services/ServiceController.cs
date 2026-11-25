using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Gaia.Models;

namespace Inanna.Services;

public interface IServiceController
{
    ConfiguredValueTaskAwaitable<Result> RefreshServicesAsync(CancellationToken ct);
}

public sealed class ServiceController : IServiceController
{
    public ServiceController(IEnumerable<IUiService> uiServices)
    {
        _uiServices = uiServices;
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshServicesAsync(CancellationToken ct)
    {
        return RefreshServicesCore(ct).ConfigureAwait(false);
    }

    private async ValueTask<Result> RefreshServicesCore(CancellationToken ct)
    {
        var results = await TaskHelper.WhenAllAsync(
            _uiServices.Select(s => s.RefreshServiceAsync(ct)).ToArray(),
            ct
        );

        var errors = results
            .Select(x =>
                x switch
                {
                    Success => null,
                    ValidationError error => error,
                }
            )
            .WhereNotNull()
            .ToArray();

        if (errors.Length != 0)
        {
            return new ValidationErrors(errors);
        }

        return Result.Ok;
    }

    private readonly IEnumerable<IUiService> _uiServices;
}
