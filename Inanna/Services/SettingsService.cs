using System.Runtime.CompilerServices;

namespace Inanna.Services;

public interface ISettingsService<T>
{
    ConfiguredValueTaskAwaitable<T> GetSettingsAsync(CancellationToken ct);
    ConfiguredValueTaskAwaitable SaveSettingsAsync(T settings, CancellationToken ct);
    T GetSettings();
    void SaveSettings(T settings);
}
