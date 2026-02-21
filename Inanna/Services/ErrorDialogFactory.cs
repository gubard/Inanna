using Gaia.Models;
using Gaia.Services;
using Inanna.Ui;

namespace Inanna.Services;

public interface IErrorDialogFactory
    : IFactory<Exception[], DialogViewModel>,
        IFactory<ValidationError[], DialogViewModel>;
