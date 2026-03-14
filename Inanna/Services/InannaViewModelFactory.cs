using Avalonia.Collections;
using Gaia.Models;
using Gaia.Services;
using Inanna.Models;
using Inanna.Ui;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Inanna.Services;

public interface IInannaViewModelFactory
{
    LinearBarcodeGeneratorViewModel CreateLinearBarcodeGenerator();
    LinearBarcodeViewModel CreateLinearBarcode();
    ValidationErrorsViewModel CreateValidationErrors(params Span<ValidationError> errors);
    ExceptionViewModel CreateException(params Span<Exception> exceptions);
    AdaptiveButtonsViewModel CreateAdaptiveButtons(IAvaloniaReadOnlyList<InannaCommand> commands);
    ChangeOrderViewModel CreateChangeOrder(IEnumerable<IOrderedItem> items);
    ServiceOfflineStatusViewModel CreateServiceOfflineStatus(IServiceState state);
    StackViewModel CreateStack();
    LogsViewModel CreateLogs();
}

public sealed class InannaViewModelFactory : IInannaViewModelFactory
{
    public InannaViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ServiceOfflineStatusViewModel CreateServiceOfflineStatus(IServiceState state)
    {
        return new(
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            state,
            _serviceProvider.GetService<InannaCommands>()
        );
    }

    public StackViewModel CreateStack()
    {
        return new(_serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    public LogsViewModel CreateLogs()
    {
        return new(_serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    public LinearBarcodeGeneratorViewModel CreateLinearBarcodeGenerator()
    {
        return new(
            _serviceProvider.GetService<ILinearBarcodeSerializerFactory>(),
            this,
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public LinearBarcodeViewModel CreateLinearBarcode()
    {
        return new(_serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    public ValidationErrorsViewModel CreateValidationErrors(params Span<ValidationError> errors)
    {
        return new(
            _serviceProvider.GetService<IClipboardService>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            errors
        );
    }

    public ExceptionViewModel CreateException(params Span<Exception> exceptions)
    {
        return new(
            _serviceProvider.GetService<IClipboardService>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            exceptions
        );
    }

    public AdaptiveButtonsViewModel CreateAdaptiveButtons(
        IAvaloniaReadOnlyList<InannaCommand> commands
    )
    {
        return new(commands, _serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    public ChangeOrderViewModel CreateChangeOrder(IEnumerable<IOrderedItem> items)
    {
        return new(items, _serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    private readonly IServiceProvider _serviceProvider;
}
