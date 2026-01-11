using System.Runtime.CompilerServices;
using Gaia.Models;
using Gaia.Services;
using Inanna.Models;
using Nestor.Db.Models;
using Nestor.Db.Services;

namespace Inanna.Services;

public interface IUiService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse>
    : IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>,
        IHealthCheck
    where TGetResponse : IValidationErrors, new()
    where TPostResponse : IValidationErrors, new()
{
    string ServiceName { get; }

    ConfiguredValueTaskAwaitable<TPostResponse> UpdateEventsAsync(CancellationToken ct);
}

public abstract class UiService<
    TGetRequest,
    TPostRequest,
    TGetResponse,
    TPostResponse,
    THttpService,
    TEfService,
    TCache
> : IUiService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : IValidationErrors, IResponse, new()
    where TPostResponse : IValidationErrors, IPostResponse, new()
    where TGetRequest : new()
    where THttpService : IHttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TEfService : IDbService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TPostRequest : IPostRequest, new()
    where TCache : IUiCache<TPostRequest, TGetResponse, IMemoryCache<TPostRequest, TGetResponse>>
{
    public string ServiceName { get; }

    public ConfiguredValueTaskAwaitable<TPostResponse> UpdateEventsAsync(CancellationToken ct)
    {
        return PostCore(Guid.NewGuid(), new(), ct).ConfigureAwait(false);
    }

    public virtual ConfiguredValueTaskAwaitable<TGetResponse> GetAsync(
        TGetRequest request,
        CancellationToken ct
    )
    {
        return GetCore(request, ct).ConfigureAwait(false);
    }

    public virtual ConfiguredValueTaskAwaitable<TPostResponse> PostAsync(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        return PostCore(idempotentId, request, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<IValidationErrors> HealthCheckAsync(CancellationToken ct)
    {
        return HealthCheckCore(ct).ConfigureAwait(false);
    }

    protected UiService(
        THttpService httpService,
        TEfService dbService,
        AppState appState,
        TCache uiCache,
        INavigator navigator,
        string serviceName
    )
    {
        _httpService = httpService;
        _dbService = dbService;
        _appState = appState;
        _uiCache = uiCache;
        _navigator = navigator;
        ServiceName = serviceName;
    }

    private readonly THttpService _httpService;
    private readonly TEfService _dbService;
    private readonly AppState _appState;
    private readonly TCache _uiCache;
    private readonly INavigator _navigator;

    private async ValueTask<IValidationErrors> HealthCheckCore(CancellationToken ct)
    {
        var errors = await _httpService.HealthCheckAsync(ct);

        if (errors.ValidationErrors.Count == 0)
        {
            _appState.SetServiceMode(ServiceName, ServiceMode.Online);

            return errors;
        }

        _appState.SetServiceMode(ServiceName, ServiceMode.Offline);

        return errors;
    }

    private async ValueTask<TPostResponse> PostCore(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        var mode = _appState.GetServiceMode(ServiceName);

        switch (mode)
        {
            case ServiceMode.Online:
            {
                await _uiCache.UpdateAsync(request, ct);
                var events = await _dbService.GetEventsAsync(ct);
                request.Events = events;
                var response = await _httpService.PostAsync(idempotentId, request, ct);

                if (response.IsEventSaved)
                {
                    await _dbService.ClearEventsAsync(ct);
                }

                if (response.ValidationErrors.OfType<ExceptionsValidationError>().Any())
                {
                    var r = await _dbService.PostAsync(idempotentId, request, ct);
                    response.ValidationErrors.AddRange(r.ValidationErrors);
                }

                await _navigator.RefreshCurrentViewAsync(ct);

                return response;
            }
            case ServiceMode.Offline:
            {
                var response = await _dbService.PostAsync(idempotentId, request, ct);
                await _navigator.RefreshCurrentViewAsync(ct);

                return response;
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private async ValueTask<TGetResponse> GetCore(TGetRequest request, CancellationToken ct)
    {
        var mode = _appState.GetServiceMode(ServiceName);

        switch (mode)
        {
            case ServiceMode.Online:
            {
                var response = await _httpService.GetAsync(request, ct);
                await _uiCache.UpdateAsync(response, ct);

                return response;
            }
            case ServiceMode.Offline:
            {
                var response = await _dbService.GetAsync(request, ct);
                await _uiCache.MemoryCache.UpdateAsync(response, ct);

                return response;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }
}
