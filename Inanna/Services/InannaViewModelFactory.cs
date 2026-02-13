using Avalonia.Collections;
using Gaia.Models;
using Gaia.Services;
using Inanna.Models;
using Inanna.Ui;

namespace Inanna.Services;

public interface IInannaViewModelFactory
{
    LinearBarcodeGeneratorViewModel CreateLinearBarcodeGenerator();
    LinearBarcodeViewModel CreateLinearBarcode();
    ValidationErrorsViewModel CreateValidationErrors(ValidationError[] errors);
    ExceptionViewModel CreateException(params Span<Exception> exceptions);
    AdaptiveButtonsViewModel CreateAdaptiveButtons(IAvaloniaReadOnlyList<InannaCommand> commands);
}

public sealed class InannaViewModelFactory : IInannaViewModelFactory
{
    public InannaViewModelFactory(
        IClipboardService clipboardService,
        ILinearBarcodeSerializerFactory linearBarcodeSerializerFactory
    )
    {
        _clipboardService = clipboardService;
        _linearBarcodeSerializerFactory = linearBarcodeSerializerFactory;
    }

    public LinearBarcodeGeneratorViewModel CreateLinearBarcodeGenerator()
    {
        return new(_linearBarcodeSerializerFactory, this);
    }

    public LinearBarcodeViewModel CreateLinearBarcode()
    {
        return new();
    }

    public ValidationErrorsViewModel CreateValidationErrors(ValidationError[] errors)
    {
        return new(_clipboardService, errors);
    }

    public ExceptionViewModel CreateException(params Span<Exception> exceptions)
    {
        return new(_clipboardService, exceptions);
    }

    public AdaptiveButtonsViewModel CreateAdaptiveButtons(
        IAvaloniaReadOnlyList<InannaCommand> commands
    )
    {
        return new(commands);
    }

    private readonly IClipboardService _clipboardService;
    private readonly ILinearBarcodeSerializerFactory _linearBarcodeSerializerFactory;
}
