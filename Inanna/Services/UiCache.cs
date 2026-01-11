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

public class UiCache<TPostRequest, TGetResponse, TDbCache, TMemoryCache>
    : IUiCache<TPostRequest, TGetResponse, TMemoryCache>
    where TDbCache : IDbCache<TPostRequest, TGetResponse>
    where TMemoryCache : IMemoryCache<TPostRequest, TGetResponse>
{
    public UiCache(TDbCache dbCache, TMemoryCache memoryCache)
    {
        DbCache = dbCache;
        MemoryCache = memoryCache;
    }

    public TMemoryCache MemoryCache { get; }

    public ConfiguredValueTaskAwaitable UpdateAsync(TPostRequest source, CancellationToken ct)
    {
        return TaskHelper.WhenAllAsync(
            DbCache.UpdateAsync(source, ct),
            MemoryCache.UpdateAsync(source, ct)
        );
    }

    public ConfiguredValueTaskAwaitable UpdateAsync(TGetResponse source, CancellationToken ct)
    {
        return TaskHelper.WhenAllAsync(
            DbCache.UpdateAsync(source, ct),
            MemoryCache.UpdateAsync(source, ct)
        );
    }

    protected readonly TDbCache DbCache;
}
