using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using Gaia.Helpers;
using Inanna.Helpers;

namespace Inanna.Services;

public interface IClipboardService
{
    ConfiguredValueTaskAwaitable SetTextAsync(string? text, CancellationToken ct);
}

public sealed class AvaloniaClipboardService : IClipboardService
{
    public AvaloniaClipboardService(Application app)
    {
        _app = app;
    }

    public ConfiguredValueTaskAwaitable SetTextAsync(string? text, CancellationToken ct)
    {
        return SetTextCore(text, ct).ConfigureAwait(false);
    }

    private readonly Application _app;

    private async ValueTask SetTextCore(string? text, CancellationToken ct)
    {
        var topLevel = _app.GetTopLevel().ThrowIfNull();
        ct.ThrowIfCancellationRequested();

        await Dispatcher.UIThread.InvokeAsync(async () =>
            await topLevel.Clipboard.ThrowIfNull().SetTextAsync(text)
        );
    }
}
