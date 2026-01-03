namespace Inanna.Services;

public interface ISettingsService<T>
{
    ValueTask<T> GetSettingsAsync(CancellationToken ct);
    ValueTask SaveSettingsAsync(T settings, CancellationToken ct);
    T GetSettings();
    void SaveSettings(T settings);
}
