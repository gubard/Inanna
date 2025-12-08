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
        THttpService, TEfService> :
    IUiService<TGetRequest, TPostRequest, TGetResponse, TPostResponse>
    where TGetResponse : IValidationErrors, IResponse, new()
    where TPostResponse : IValidationErrors, IResponse, new()
    where TGetRequest : IGetRequest, new()
    where THttpService : IService<TGetRequest, TPostRequest, TGetResponse,
        TPostResponse>
    where TEfService : IEfService<TGetRequest, TPostRequest, TGetResponse,
        TPostResponse>
    where TPostRequest : IPostRequest
{
    private readonly THttpService _service;
    private readonly TEfService _efService;
    private readonly AppState _appState;
    private long _lastHttpId;

    protected UiService(THttpService service, TEfService efService,
        AppState appState)
    {
        _service = service;
        _efService = efService;
        _appState = appState;
        _lastHttpId = -1;
    }

    public async ValueTask<TGetResponse> GetAsync(TGetRequest request,
        CancellationToken ct)
    {
        switch (_appState.Mode)
        {
            case AppMode.Online:
            {
                await InitAsync(ct);

                return await _service.GetAsync(request, ct);
            }
            case AppMode.Offline:
                return await _efService.GetAsync(request, ct);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async ValueTask<TPostResponse> PostAsync(TPostRequest request,
        CancellationToken ct)
    {
        switch (_appState.Mode)
        {
            case AppMode.Online:
            {
                await InitAsync(ct);
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
        if (_lastHttpId != -1)
        {
            return;
        }

        var request = new TGetRequest();
        var lastLocalId = await _efService.GetLastIdAsync(ct);
        request.LastId = lastLocalId;
        var response = await _service.GetAsync(request, ct);

        if (response.Events.Length == 0)
        {
            return;
        }

        await _efService.SaveEventsAsync(response.Events, ct);
        _lastHttpId = response.Events.Max(x => x.Id);
    }
}