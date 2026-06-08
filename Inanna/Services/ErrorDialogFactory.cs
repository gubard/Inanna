using Gaia.Models;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Ui;

namespace Inanna.Services;

public interface IErrorDialogFactory
    : IFactory<Exception[], object>,
        IFactory<ValidationError[], object>;

public sealed class ErrorDialogFactory : IErrorDialogFactory
{
    public ErrorDialogFactory(IInannaViewModelFactory factory)
    {
        _factory = factory;
    }

    public object Create(Exception[] input)
    {
        return _factory.CreateException(input);
    }

    public object Create(ValidationError[] input)
    {
        return _factory.CreateValidationErrors(input);
    }

    private readonly IInannaViewModelFactory _factory;
}
