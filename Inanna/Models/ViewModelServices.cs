using Inanna.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Inanna.Models;

public sealed class ViewModelServices
{
    public ISafeExecuteWrapper SafeExecuteWrapper => _safeExecuteWrapper.Value;
    public IDialogService DialogService => _dialogService.Value;
    public IAppResourceService AppResourceService => _appResourceService.Value;
    public IErrorDialogFactory ErrorDialogFactory => _errorDialogFactory.Value;

    public ViewModelServices(IServiceProvider serviceProvider)
    {
        _safeExecuteWrapper = new(serviceProvider.GetRequiredService<ISafeExecuteWrapper>);
        _dialogService = new(serviceProvider.GetRequiredService<IDialogService>);
        _appResourceService = new(serviceProvider.GetRequiredService<IAppResourceService>);
        _errorDialogFactory = new(serviceProvider.GetRequiredService<IErrorDialogFactory>);
    }

    private readonly Lazy<ISafeExecuteWrapper> _safeExecuteWrapper;
    private readonly Lazy<IDialogService> _dialogService;
    private readonly Lazy<IAppResourceService> _appResourceService;
    private readonly Lazy<IErrorDialogFactory> _errorDialogFactory;
}
