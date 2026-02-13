using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed partial class ExceptionViewModel : ViewModelBase
{
    public ExceptionViewModel(IClipboardService clipboardService, params Span<Exception> exceptions)
    {
        _clipboardService = clipboardService;
        _exceptions = new(exceptions.ToArray());
    }

    public IAvaloniaReadOnlyList<Exception> Exceptions => _exceptions;

    private readonly AvaloniaList<Exception> _exceptions;
    private readonly IClipboardService _clipboardService;

    [RelayCommand]
    private async Task CopyAsync(CancellationToken ct)
    {
        await WrapCommandAsync(
            () =>
                _clipboardService.SetTextAsync(
                    _exceptions.Select(x => x.ToString()).JoinString(Environment.NewLine),
                    ct
                ),
            ct
        );
    }
}
