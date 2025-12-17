using Avalonia;
using Gaia.Helpers;
using Inanna.Helpers;

namespace Inanna.Services;

public interface IClipboardService
{
    ValueTask SetTextAsync(string? text, CancellationToken ct);
}

public class AvaloniaClipboardService : IClipboardService
{
    private readonly Application _application;

    public AvaloniaClipboardService(Application application)
    {
        _application = application;
    }

    public async ValueTask SetTextAsync(string? text, CancellationToken ct)
    {
        var topLevel = _application.GetTopLevel().ThrowIfNull();
        ct.ThrowIfCancellationRequested();
        await topLevel.Clipboard.ThrowIfNull().SetTextAsync(text);
    }
}
