using System.Runtime.CompilerServices;
using Avalonia.Threading;
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
    where TPostResponse : IValidationErrors, IResponse, new()
    where TGetRequest : new()
    where THttpService : IHttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TEfService : IDbService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TPostRequest : IPostRequest
    where TCache : IUiCache<TPostRequest, TGetResponse, IMemoryCache<TPostRequest, TGetResponse>>
{
    public string ServiceName { get; }

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

    public TPostResponse Post(Guid idempotentId, TPostRequest request)
    {
        _uiCache.Update(request);
        var response = PostCore(idempotentId, request);
        _navigator.RefreshCurrentView();

        return response;
    }

    public TGetResponse Get(TGetRequest request)
    {
        var mode = _appState.GetServiceMode(ServiceName);

        switch (mode)
        {
            case ServiceMode.Online:
            {
                var response = _httpService.Get(request);
                _uiCache.Update(response);

                return response;
            }
            case ServiceMode.Offline:
            {
                var response = _dbService.Get(request);
                _uiCache.MemoryCache.Update(response);

                return response;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    public ConfiguredValueTaskAwaitable<bool> HealthCheckAsync(CancellationToken ct)
    {
        return HealthCheckCore(ct).ConfigureAwait(false);
    }

    public bool HealthCheck()
    {
        if (_httpService.HealthCheck())
        {
            _appState.SetServiceMode(ServiceName, ServiceMode.Online);

            return true;
        }

        _appState.SetServiceMode(ServiceName, ServiceMode.Offline);

        return false;
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

    private async ValueTask<bool> HealthCheckCore(CancellationToken ct)
    {
        if (await _httpService.HealthCheckAsync(ct))
        {
            _appState.SetServiceMode(ServiceName, ServiceMode.Online);

            return true;
        }

        _appState.SetServiceMode(ServiceName, ServiceMode.Offline);

        return false;
    }

    private async ValueTask<TPostResponse> PostCore(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        _uiCache.Update(request);
        var response = await PostCoreAsync(idempotentId, request, ct);
        await _navigator.RefreshCurrentViewAsync(ct);

        return response;
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

    private TPostResponse PostCore(Guid idempotentId, TPostRequest request)
    {
        var mode = _appState.GetServiceMode(ServiceName);

        switch (mode)
        {
            case ServiceMode.Online:
            {
                _uiCache.Update(request);
                var events = _dbService.GetEvents();
                request.Events = events;
                var response = _httpService.Post(idempotentId, request);

                if (request.Events.Length != 0)
                {
                    _dbService.ClearEvents();
                }

                return response;
            }
            case ServiceMode.Offline:
                return _dbService.Post(idempotentId, request);

            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private async ValueTask<TPostResponse> PostCoreAsync(
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

                if (request.Events.Length != 0)
                {
                    await _dbService.ClearEventsAsync(ct);
                }

                return response;
            }
            case ServiceMode.Offline:
                return await _dbService.PostAsync(idempotentId, request, ct);

            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }
}
