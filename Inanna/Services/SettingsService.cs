namespace Inanna.Services;

public interface ISettingsService<T>
{
    ValueTask<T> GetSettingsAsync(CancellationToken ct);
    ValueTask SaveSettingsAsync(T settings, CancellationToken ct);
}
