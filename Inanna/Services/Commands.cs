using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Threading;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Inanna.Models;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Inanna.Services;

public abstract class Commands
{
    protected readonly IServiceProvider ServiceProvider;

    protected Commands(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    protected Lazy<ICommand> CreateLazyCommand<T>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand<T>(
        Func<T, CancellationToken, ValueTask> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand(
        Func<CancellationToken, ConfiguredValueTaskAwaitable> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand(
        Func<CancellationToken, ValueTask> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand(
        Func<CancellationToken, ValueTask<Result>> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand<T>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable<Result<T>>> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand<T>(
        Func<T, CancellationToken, ValueTask<Result<T>>> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }
}

public sealed class InannaCommands : Commands
{
    public InannaCommands(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
        _showPaneCommand = CreateLazyCommand(_ =>
        {
            ServiceProvider.GetService<IPaneService>().ShowPane();

            return TaskHelper.ConfiguredCompletedTask;
        });

        _hidePaneCommand = CreateLazyCommand(_ =>
        {
            ServiceProvider.GetService<IPaneService>().HidePane();

            return TaskHelper.ConfiguredCompletedTask;
        });

        _openLinkCommand = CreateLazyCommand<Uri>(
            ServiceProvider.GetService<IOpenerLink>().OpenLinkAsync
        );

        _navigateToCommand = CreateLazyCommand<Type>(
            (type, ct) =>
                ServiceProvider
                    .GetService<INavigator>()
                    .NavigateToAsync(DiHelper.ServiceProvider.GetService(type), ct)
        );

        _switchServiceModeCommand = CreateLazyCommand<IServiceState>(
            async (state, ct) =>
            {
                if (state.Mode == ServiceMode.Online)
                {
                    Dispatcher.UIThread.Post(() => state.Mode = ServiceMode.Offline);

                    return state;
                }

                var result = await state.HealthCheckAsync(ct);

                if (result is ValidationError error)
                {
                    return error;
                }

                await ServiceProvider.GetService<INavigator>().RefreshCurrentViewAsync(ct);

                return state;
            }
        );
    }

    public ICommand ShowPaneCommand => _showPaneCommand.Value;
    public ICommand HidePaneCommand => _hidePaneCommand.Value;
    public ICommand NavigateToCommand => _navigateToCommand.Value;
    public ICommand OpenLinkCommand => _openLinkCommand.Value;
    public ICommand SwitchServiceModeCommand => _switchServiceModeCommand.Value;

    private readonly Lazy<ICommand> _navigateToCommand;
    private readonly Lazy<ICommand> _openLinkCommand;
    private readonly Lazy<ICommand> _switchServiceModeCommand;
    private readonly Lazy<ICommand> _showPaneCommand;
    private readonly Lazy<ICommand> _hidePaneCommand;
}
