using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Input.Platform;
using Gaia.Helpers;
using Inanna.Helpers;

namespace Inanna.Services;

public interface IClipboardService
{
    ConfiguredValueTaskAwaitable SetTextAsync(string? text, CancellationToken ct);
}

public sealed class AvaloniaClipboardService : IClipboardService
{
    public AvaloniaClipboardService(Application application)
    {
        _application = application;
    }

    public ConfiguredValueTaskAwaitable SetTextAsync(string? text, CancellationToken ct)
    {
        return SetTextCore(text, ct).ConfigureAwait(false);
    }

    private readonly Application _application;

    private async ValueTask SetTextCore(string? text, CancellationToken ct)
    {
        var topLevel = _application.GetTopLevel().ThrowIfNull();
        ct.ThrowIfCancellationRequested();
        await topLevel.Clipboard.ThrowIfNull().SetTextAsync(text);
    }
}
