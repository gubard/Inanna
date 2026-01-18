using Gaia.Models;
using Inanna.Ui;

namespace Inanna.Services;

public interface IInannaViewModelFactory
{
    ValidationErrorsViewModel CreateValidationErrors(ValidationError[] errors);
    ExceptionViewModel CreateException(params Span<Exception> exceptions);
}

public sealed class InannaViewModelFactory : IInannaViewModelFactory
{
    private readonly IClipboardService _clipboardService;

    public InannaViewModelFactory(IClipboardService clipboardService)
    {
        _clipboardService = clipboardService;
    }

    public ValidationErrorsViewModel CreateValidationErrors(ValidationError[] errors)
    {
        return new(_clipboardService, errors);
    }

    public ExceptionViewModel CreateException(params Span<Exception> exceptions)
    {
        return new(_clipboardService, exceptions);
    }
}
