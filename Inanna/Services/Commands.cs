using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Threading;
using Gaia.Helpers;
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
        return new Lazy<ICommand>(() =>
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
        return new Lazy<ICommand>(() =>
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
        return new Lazy<ICommand>(() =>
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
        return new Lazy<ICommand>(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand<TValidationErrors>(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors
    {
        return new Lazy<ICommand>(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand<TValidationErrors>(
        Func<CancellationToken, ValueTask<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors
    {
        return new Lazy<ICommand>(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand<T, TValidationErrors>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors
    {
        return new Lazy<ICommand>(() =>
            ServiceProvider
                .GetService<ICommandFactory>()
                .CreateCommand(func, isBackground, canCancel)
        );
    }

    protected Lazy<ICommand> CreateLazyCommand<T, TValidationErrors>(
        Func<T, CancellationToken, ValueTask<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors
    {
        return new Lazy<ICommand>(() =>
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
        _openLinkCommand = CreateLazyCommand<Uri>(
            ServiceProvider.GetService<IOpenerLink>().OpenLinkAsync
        );

        _navigateToCommand = CreateLazyCommand<Type>(
            (type, ct) =>
                ServiceProvider
                    .GetService<INavigator>()
                    .NavigateToAsync(DiHelper.ServiceProvider.GetService(type), ct)
        );

        _switchServiceModeCommand = CreateLazyCommand<IServiceState, IValidationErrors>(
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

                await ServiceProvider.GetService<INavigator>().RefreshCurrentViewAsync(ct);

                return errors;
            }
        );
    }

    public ICommand NavigateToCommand => _navigateToCommand.Value;
    public ICommand OpenLinkCommand => _openLinkCommand.Value;
    public ICommand SwitchServiceModeCommand => _switchServiceModeCommand.Value;

    private readonly Lazy<ICommand> _navigateToCommand;
    private readonly Lazy<ICommand> _openLinkCommand;
    private readonly Lazy<ICommand> _switchServiceModeCommand;
}
