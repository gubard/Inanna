using Avalonia;
using Gaia.Helpers;
using Inanna.Helpers;

namespace Inanna.Services;

public interface IClipboardService
{
    Task SetTextAsync(string? text, CancellationToken ct);
}

public class AvaloniaClipboardService : IClipboardService
{
    private readonly Application _application;

    public AvaloniaClipboardService(Application application)
    {
        _application = application;
    }

    public Task SetTextAsync(string? text, CancellationToken ct)
    {
        var topLevel = _application.GetTopLevel().ThrowIfNull();

        return topLevel.Clipboard.ThrowIfNull().SetTextAsync(text);
    }
}