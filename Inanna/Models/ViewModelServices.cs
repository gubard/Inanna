using Avalonia;
using Inanna.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inanna.Models;

public sealed class ViewModelServices
{
    public ISafeExecuteWrapper SafeExecuteWrapper => _safeExecuteWrapper.Value;
    public IDialogService DialogService => _dialogService.Value;
    public IAppResourceService AppResourceService => _appResourceService.Value;
    public IErrorDialogFactory ErrorDialogFactory => _errorDialogFactory.Value;
    public Application App => _app.Value;
    public ILogger Logger => _logger.Value;

    public ViewModelServices(IServiceProvider serviceProvider)
    {
        _safeExecuteWrapper = new(serviceProvider.GetRequiredService<ISafeExecuteWrapper>);
        _dialogService = new(serviceProvider.GetRequiredService<IDialogService>);
        _appResourceService = new(serviceProvider.GetRequiredService<IAppResourceService>);
        _errorDialogFactory = new(serviceProvider.GetRequiredService<IErrorDialogFactory>);
        _app = new(serviceProvider.GetRequiredService<Application>);
        _logger = new(serviceProvider.GetRequiredService<ILogger>);
    }

    private readonly Lazy<ISafeExecuteWrapper> _safeExecuteWrapper;
    private readonly Lazy<IDialogService> _dialogService;
    private readonly Lazy<IAppResourceService> _appResourceService;
    private readonly Lazy<IErrorDialogFactory> _errorDialogFactory;
    private readonly Lazy<Application> _app;
    private readonly Lazy<ILogger> _logger;
}
