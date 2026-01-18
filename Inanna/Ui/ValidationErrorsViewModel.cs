using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Models;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public partial class ValidationErrorsViewModel : ViewModelBase
{
    public ValidationErrorsViewModel(
        IClipboardService clipboardService,
        params Span<ValidationError> errors
    )
    {
        _clipboardService = clipboardService;
        _errors = new(errors.ToArray());
    }

    public IEnumerable<ValidationError> Errors => _errors;

    private readonly IClipboardService _clipboardService;
    private readonly AvaloniaList<ValidationError> _errors;

    [RelayCommand]
    private async Task CopyAsync(CancellationToken ct)
    {
        await WrapCommandAsync(
            () =>
                _clipboardService.SetTextAsync(
                    _errors
                        .Select(x => x.ToString())
                        .WhereNotNull()
                        .JoinString(Environment.NewLine),
                    ct
                ),
            ct
        );
    }
}
