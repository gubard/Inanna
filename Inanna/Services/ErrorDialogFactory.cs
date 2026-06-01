using Gaia.Models;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Ui;

namespace Inanna.Services;

public interface IErrorDialogFactory
    : IFactory<Exception[], DialogViewModel>,
        IFactory<ValidationError[], DialogViewModel>;

public sealed class ErrorDialogFactory : IErrorDialogFactory
{
    public ErrorDialogFactory(Gaia.Services.IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public DialogViewModel Create(Exception[] input)
    {
        return new(
            _serviceProvider
                .GetService<IAppResourceService>()
                .GetResource<string>("Lang.Error")
                .DispatchToDialogHeader(),
            _serviceProvider.GetService<IInannaViewModelFactory>().CreateException(input),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider.GetService<IDialogService>().OkButton
        );
    }

    public DialogViewModel Create(ValidationError[] input)
    {
        return new(
            _serviceProvider
                .GetService<IAppResourceService>()
                .GetResource<string>("Lang.Error")
                .DispatchToDialogHeader(),
            _serviceProvider.GetService<IInannaViewModelFactory>().CreateValidationErrors(input),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider.GetService<IDialogService>().OkButton
        );
    }

    private readonly Gaia.Services.IServiceProvider _serviceProvider;
}
