using Avalonia.Collections;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Models;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed partial class ValidationErrorsViewModel : ViewModelBase
{
    public ValidationErrorsViewModel(
        IClipboardService clipboardService,
        ViewModelServices services,
        params Span<ValidationError> validationErrors
    )
        : base(services)
    {
        _clipboardService = clipboardService;
        _validationErrors = new(validationErrors.ToArray());
    }

    public IEnumerable<ValidationError> ValidationErrors => _validationErrors;

    private readonly IClipboardService _clipboardService;
    private readonly AvaloniaList<ValidationError> _validationErrors;

    [RelayCommand]
    private async Task CopyAsync(CancellationToken ct)
    {
        await WrapCommandAsync(
            () =>
                _clipboardService.SetTextAsync(
                    _validationErrors
                        .Select(x => x.ToString())
                        .WhereNotNull()
                        .JoinString(Environment.NewLine),
                    ct
                ),
            ct
        );
    }
}
