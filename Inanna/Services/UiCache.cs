using System.Runtime.CompilerServices;
using Gaia.Helpers;
using Gaia.Services;
using Nestor.Db.Services;

namespace Inanna.Services;

public interface IUiCache<in TPostRequest, in TGetResponse, out TMemoryCache>
    : ICache<TPostRequest, TGetResponse>
    where TMemoryCache : IMemoryCache<TPostRequest, TGetResponse>
{
    public TMemoryCache MemoryCache { get; }
}

public abstract class UiCache<TPostRequest, TGetResponse, TDbCache, TMemoryCache>
    : IUiCache<TPostRequest, TGetResponse, TMemoryCache>
    where TDbCache : IDbCache<TPostRequest, TGetResponse>
    where TMemoryCache : IMemoryCache<TPostRequest, TGetResponse>
{
    protected UiCache(TDbCache dbCache, TMemoryCache memoryCache)
    {
        _dbCache = dbCache;
        MemoryCache = memoryCache;
    }

    public TMemoryCache MemoryCache { get; }

    public ConfiguredValueTaskAwaitable UpdateAsync(TPostRequest source, CancellationToken ct)
    {
        return TaskHelper.WhenAllAsync(
            [_dbCache.UpdateAsync(source, ct), MemoryCache.UpdateAsync(source, ct)],
            ct
        );
    }

    public ConfiguredValueTaskAwaitable UpdateAsync(TGetResponse source, CancellationToken ct)
    {
        return TaskHelper.WhenAllAsync(
            [_dbCache.UpdateAsync(source, ct), MemoryCache.UpdateAsync(source, ct)],
            ct
        );
    }

    private readonly TDbCache _dbCache;
}
