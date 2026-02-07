using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Models;
using Gaia.Services;
using Inanna.Models;
using Nestor.Db.Models;
using Nestor.Db.Services;

namespace Inanna.Services;

public interface IUiService
{
    ConfiguredValueTaskAwaitable<IValidationErrors> RefreshServiceAsync(CancellationToken ct);
}

public interface IUiService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse>
    : IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>,
        IServiceState,
        IUiService
    where TGetResponse : IResponse, new()
    where TPostResponse : IPostResponse, new()
{
    ConfiguredValueTaskAwaitable<TPostResponse> UpdateEventsAsync(CancellationToken ct);
}

public abstract partial class UiService<
    TGetRequest,
    TPostRequest,
    TGetResponse,
    TPostResponse,
    THttpService,
    TDbService,
    TCache
> : ObservableObject, IUiService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : IResponse, new()
    where TPostResponse : IPostResponse, new()
    where TGetRequest : new()
    where THttpService : IHttpService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TDbService : IDbService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
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

    public ConfiguredValueTaskAwaitable<IValidationErrors> RefreshServiceAsync(CancellationToken ct)
    {
        return RefreshServiceCore(ct).ConfigureAwait(false);
    }

    protected UiService(
        THttpService httpService,
        TDbService dbService,
        TCache uiCache,
        INavigator navigator,
        string serviceName,
        IResponseHandler responseHandler
    )
    {
        _httpService = httpService;
        _dbService = dbService;
        _uiCache = uiCache;
        _navigator = navigator;
        ServiceName = serviceName;
        _responseHandler = responseHandler;
    }

    protected abstract TGetRequest CreateGetRequestRefresh();

    [ObservableProperty]
    private ServiceMode _mode;

    private readonly THttpService _httpService;
    private readonly TDbService _dbService;
    private readonly TCache _uiCache;
    private readonly INavigator _navigator;
    private readonly IResponseHandler _responseHandler;

    private async ValueTask<IValidationErrors> HealthCheckCore(CancellationToken ct)
    {
        var errors = await _httpService.HealthCheckAsync(ct);

        if (errors.ValidationErrors.Count == 0)
        {
            Mode = ServiceMode.Online;

            return errors;
        }

        Mode = ServiceMode.Offline;

        return errors;
    }

    private async ValueTask<IValidationErrors> RefreshServiceCore(CancellationToken ct)
    {
        var request = CreateGetRequestRefresh();
        var response = await _dbService.GetAsync(request, ct);
        await _uiCache.UpdateAsync(response, ct);

        return response;
    }

    private async ValueTask<TPostResponse> PostCore(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        switch (Mode)
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

                await _responseHandler.HandleResponseAsync(response, ct);
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
                throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null);
        }
    }

    private async ValueTask<TGetResponse> GetCore(TGetRequest request, CancellationToken ct)
    {
        switch (Mode)
        {
            case ServiceMode.Online:
            {
                var response = await _httpService.GetAsync(request, ct);
                await _uiCache.UpdateAsync(response, ct);
                await _responseHandler.HandleResponseAsync(response, ct);

                return response;
            }
            case ServiceMode.Offline:
            {
                var response = await _dbService.GetAsync(request, ct);
                await _uiCache.MemoryCache.UpdateAsync(response, ct);

                return response;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null);
        }
    }
}
