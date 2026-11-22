using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Models;
using Gaia.Services;
using Inanna.Models;
using Inanna.Ui;
using Nestor.Db.Models;
using Nestor.Db.Services;

namespace Inanna.Services;

public interface IUiService
{
    ConfiguredValueTaskAwaitable<Result> RefreshServiceAsync(CancellationToken ct);
}

public interface IUiService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse>
    : IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>,
        IServiceState,
        IUiService
    where TGetResponse : IResponse, new()
    where TPostResponse : IPostResponse, new();

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

    public virtual ConfiguredValueTaskAwaitable<Result<TGetResponse>> GetAsync(
        TGetRequest request,
        CancellationToken ct
    )
    {
        return GetCore(request, ct).ConfigureAwait(false);
    }

    public virtual ConfiguredValueTaskAwaitable<Result<TPostResponse>> PostAsync(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        return PostCore(idempotentId, request, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> HealthCheckAsync(CancellationToken ct)
    {
        return HealthCheckCore(ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable<Result> RefreshServiceAsync(CancellationToken ct)
    {
        return RefreshServiceCore(ct).ConfigureAwait(false);
    }

    protected readonly TDbService DbService;
    protected readonly TCache UiCache;

    protected UiService(
        THttpService httpService,
        TDbService dbService,
        TCache uiCache,
        INavigator navigator,
        string serviceName,
        IStatusBarService statusBarService,
        IInannaViewModelFactory factory
    )
    {
        _httpService = httpService;
        DbService = dbService;
        UiCache = uiCache;
        _navigator = navigator;
        ServiceName = serviceName;
        _statusBarService = statusBarService;
        _viewModel = factory.CreateServiceOfflineStatus(this);
    }

    protected abstract ValueTask<Result> RefreshServiceCore(CancellationToken ct);

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(Mode))
        {
            switch (Mode)
            {
                case ServiceMode.Online:
                    _statusBarService.RemoveStatus(_viewModel);
                    break;
                case ServiceMode.Offline:
                    _statusBarService.AddStatus(_viewModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [ObservableProperty]
    public partial ServiceMode Mode { get; set; }

    private readonly ServiceOfflineStatusViewModel _viewModel;
    private readonly THttpService _httpService;
    private readonly INavigator _navigator;
    private readonly IStatusBarService _statusBarService;

    private CancellationTokenSource? _cts;

    private async ValueTask<Result> HealthCheckCore(CancellationToken ct)
    {
        var result = await _httpService.HealthCheckAsync(ct);

        Mode = result switch
        {
            Success => ServiceMode.Online,
            ValidationError => ServiceMode.Offline,
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };

        return result;
    }

    private async ValueTask<Result<TPostResponse>> PostCore(
        Guid idempotentId,
        TPostRequest request,
        CancellationToken ct
    )
    {
        if (_cts is not null)
        {
            await _cts.CancelAsync();
        }

        var cts = new CancellationTokenSource();
        _cts = cts;

        await using var fin = new FinallyAsync(async () =>
        {
            if (!cts.IsCancellationRequested)
            {
                await _navigator.RefreshCurrentViewAsync(cts.Token);
            }
        });

        switch (Mode)
        {
            case ServiceMode.Online:
            {
                await UiCache.UpdateAsync(request, ct);
                var events = await DbService.GetEventsAsync(ct);
                request.Events = events;
                var response = await _httpService.PostAsync(idempotentId, request, ct);

                if (response is TPostResponse { IsEventSaved: true })
                {
                    await DbService.ClearEventsAsync(ct);
                }

                if (response is ValidationError error)
                {
                    var dbResult = await DbService.PostAsync(idempotentId, request, ct);

                    if (dbResult is ValidationError dbError)
                    {
                        return new ValidationErrors([error, dbError]);
                    }
                }

                return response;
            }
            case ServiceMode.Offline:
            {
                var response = await DbService.PostAsync(idempotentId, request, ct);

                return response;
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null);
        }
    }

    private async ValueTask<Result<TGetResponse>> GetCore(TGetRequest request, CancellationToken ct)
    {
        switch (Mode)
        {
            case ServiceMode.Online:
            {
                var response = await _httpService.GetAsync(request, ct);

                if (response is TGetResponse get)
                {
                    await UiCache.UpdateAsync(get, ct);
                }

                return response;
            }
            case ServiceMode.Offline:
            {
                var response = await DbService.GetAsync(request, ct);

                if (response is TGetResponse get)
                {
                    await UiCache.MemoryCache.UpdateAsync(get, ct);
                }

                return response;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(Mode), Mode, null);
        }
    }
}
