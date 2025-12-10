using Gaia.Services;
using Inanna.Models;
using Nestor.Db.Models;
using Nestor.Db.Services;

namespace Inanna.Services;

public interface
    IUiService<in TGetRequest, in TPostRequest, TGetResponse, TPostResponse> :
    IService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : IValidationErrors, new()
    where TPostResponse : IValidationErrors, new();

public abstract class
    UiService<TGetRequest, TPostRequest, TGetResponse, TPostResponse,
        THttpService, TEfService, TCache> :
    IUiService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : IValidationErrors, IResponse, new()
    where TPostResponse : IValidationErrors, IResponse, new()
    where TGetRequest : IGetRequest, new()
    where THttpService : IService<TGetRequest, TPostRequest, TGetResponse,
        TPostResponse>
    where TEfService : IEfService<TGetRequest, TPostRequest, TGetResponse,
        TPostResponse>
    where TPostRequest : IPostRequest
    where TCache : ICache<TGetResponse>, ICache<TPostRequest>
{
    private readonly THttpService _service;
    private readonly TEfService _efService;
    private readonly AppState _appState;
    private bool _inited;
    private readonly TCache _cache;
    private readonly INavigator _navigator;

    protected UiService(THttpService service, TEfService efService,
        AppState appState, TCache cache, INavigator navigator)
    {
        _service = service;
        _efService = efService;
        _appState = appState;
        _cache = cache;
        _navigator = navigator;
        _inited = false;
    }

    public virtual async ValueTask<TGetResponse> GetAsync(TGetRequest request,
        CancellationToken ct)
    {
        switch (_appState.Mode)
        {
            case AppMode.Online:
            {
                await InitAsync(ct);

                var response = await _service.GetAsync(request, ct);
                _cache.Update(response);

                return response;
            }
            case AppMode.Offline:
            {
                var response = await _efService.GetAsync(request, ct);
                _cache.Update(response);

                return response;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public virtual async ValueTask<TPostResponse> PostAsync(
        TPostRequest request,
        CancellationToken ct)
    {
        _cache.Update(request);
        await InitAsync(ct);
        var response = await PostCoreAsync(request, ct);
        await _navigator.RefreshCurrentViewAsync(ct);

        return response;
    }

    public TPostResponse Post(TPostRequest request)
    {
        _cache.Update(request);
        var response = PostCore(request);
        _navigator.RefreshCurrentView();

        return response;
    }

    public TGetResponse Get(TGetRequest request)
    {
        switch (_appState.Mode)
        {
            case AppMode.Online:
            {
                Init();

                var response = _service.Get(request);
                _cache.Update(response);

                return response;
            }
            case AppMode.Offline:
            {
                var response = _efService.Get(request);
                _cache.Update(response);

                return response;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private TPostResponse PostCore(TPostRequest request)
    {
        switch (_appState.Mode)
        {
            case AppMode.Online:
            {
                var lastLocalId = _efService.GetLastId();
                request.LastLocalId = lastLocalId;
                var response = _service.Post(request);
                _efService.SaveEvents(response.Events);

                return response;
            }
            case AppMode.Offline:
                return _efService.Post(request);

            default: throw new ArgumentOutOfRangeException();
        }
    }

    private async ValueTask<TPostResponse> PostCoreAsync(
        TPostRequest request,
        CancellationToken ct)
    {
        switch (_appState.Mode)
        {
            case AppMode.Online:
            {
                var lastLocalId = await _efService.GetLastIdAsync(ct);
                request.LastLocalId = lastLocalId;
                var response = await _service.PostAsync(request, ct);
                await _efService.SaveEventsAsync(response.Events, ct);

                return response;
            }
            case AppMode.Offline:
                return await _efService.PostAsync(request, ct);

            default: throw new ArgumentOutOfRangeException();
        }
    }

    private async ValueTask InitAsync(CancellationToken ct)
    {
        if (_inited || _appState.Mode != AppMode.Online)
        {
            return;
        }

        var request = new TGetRequest();
        var lastLocalId = await _efService.GetLastIdAsync(ct);
        request.LastId = lastLocalId;
        var response = await _service.GetAsync(request, ct);
        await _efService.SaveEventsAsync(response.Events, ct);
        _inited = true;
    }

    private void Init()
    {
        if (_inited || _appState.Mode != AppMode.Online)
        {
            return;
        }

        var request = new TGetRequest();
        var lastLocalId = _efService.GetLastId();
        request.LastId = lastLocalId;
        var response = _service.Get(request);
        _efService.SaveEvents(response.Events);
        _inited = true;
    }
}