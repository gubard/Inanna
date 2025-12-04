using Avalonia.Collections;
using Gaia.Models;
using Inanna.Models;

namespace Inanna.Ui;

public class ValidationErrorsViewModel : ViewModelBase
{
    public ValidationErrorsViewModel(params Span<ValidationError> errors)
    {
        Errors = new(errors.ToArray());
    }

    public AvaloniaList<ValidationError> Errors { get; }
}