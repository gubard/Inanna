using System.Windows.Input;
using Avalonia.Threading;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Inanna.Services;

public sealed class InannaCommands
{
    public InannaCommands(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        _openLinkCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<Uri>(_serviceProvider.GetService<IOpenerLink>().OpenLinkAsync)
        );

        _navigateToCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<Type>(
                    (type, ct) =>
                        _serviceProvider
                            .GetService<INavigator>()
                            .NavigateToAsync(DiHelper.ServiceProvider.GetService(type), ct)
                )
        );

        _switchServiceModeCommand = new Lazy<ICommand>(() =>
            _serviceProvider
                .GetService<ICommandFactory>()
                .CreateCommand<IServiceState, IValidationErrors>(
                    async (state, ct) =>
                    {
                        if (state.Mode == ServiceMode.Online)
                        {
                            Dispatcher.UIThread.Post(() => state.Mode = ServiceMode.Offline);

                            return new DefaultValidationErrors();
                        }

                        var errors = await state.HealthCheckAsync(ct);

                        if (errors.ValidationErrors.Count != 0)
                        {
                            return errors;
                        }

                        await _serviceProvider.GetService<INavigator>().RefreshCurrentViewAsync(ct);

                        return errors;
                    }
                )
        );
    }

    public ICommand NavigateToCommand => _navigateToCommand.Value;
    public ICommand OpenLinkCommand => _openLinkCommand.Value;
    public ICommand SwitchServiceModeCommand => _switchServiceModeCommand.Value;

    private readonly IServiceProvider _serviceProvider;
    private readonly Lazy<ICommand> _navigateToCommand;
    private readonly Lazy<ICommand> _openLinkCommand;
    private readonly Lazy<ICommand> _switchServiceModeCommand;
}
