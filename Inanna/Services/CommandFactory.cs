using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Services;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Inanna.Services;

public interface ICommandFactory
{
    ICommand CreateCommand<T>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable> func,
        bool isBackground = false,
        bool canCancel = true
    );

    ICommand CreateCommand<T>(
        Func<T, CancellationToken, ValueTask> func,
        bool isBackground = false,
        bool canCancel = true
    );

    ICommand CreateCommand(
        Func<CancellationToken, ConfiguredValueTaskAwaitable> func,
        bool isBackground = false,
        bool canCancel = true
    );

    ICommand CreateCommand(
        Func<CancellationToken, ValueTask> func,
        bool isBackground = false,
        bool canCancel = true
    );

    ICommand CreateCommand<TValidationErrors>(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors;

    ICommand CreateCommand<TValidationErrors>(
        Func<CancellationToken, ValueTask<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors;

    ICommand CreateCommand<T, TValidationErrors>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors;

    ICommand CreateCommand<T, TValidationErrors>(
        Func<T, CancellationToken, ValueTask<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors;
}

public sealed class CommandFactory : ICommandFactory
{
    public CommandFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICommand CreateCommand<T>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new AsyncRelayCommand<T>(
            async (parameter, ct) =>
            {
                var c = canCancel ? ct : CancellationToken.None;
                var task = _serviceProvider
                    .GetService<ISafeExecuteWrapper>()
                    .ExecuteAsync(() => func.Invoke(parameter.ThrowIfNull(), c), c);

                if (isBackground)
                {
                    return;
                }

                await task;
            }
        );
    }

    public ICommand CreateCommand<T>(
        Func<T, CancellationToken, ValueTask> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new AsyncRelayCommand<T>(
            async (value, ct) =>
            {
                var c = canCancel ? ct : CancellationToken.None;
                var task = _serviceProvider
                    .GetService<ISafeExecuteWrapper>()
                    .ExecuteAsync(
                        () => func.Invoke(value.ThrowIfNull(), c).ConfigureAwait(false),
                        c
                    );

                if (isBackground)
                {
                    return;
                }

                await task;
            }
        );
    }

    public ICommand CreateCommand(
        Func<CancellationToken, ConfiguredValueTaskAwaitable> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new AsyncRelayCommand(async ct =>
        {
            var c = canCancel ? ct : CancellationToken.None;
            var task = _serviceProvider
                .GetService<ISafeExecuteWrapper>()
                .ExecuteAsync(() => func.Invoke(c), c);

            if (isBackground)
            {
                return;
            }

            await task;
        });
    }

    public ICommand CreateCommand(
        Func<CancellationToken, ValueTask> func,
        bool isBackground = false,
        bool canCancel = true
    )
    {
        return new AsyncRelayCommand(async ct =>
        {
            var c = canCancel ? ct : CancellationToken.None;
            var task = _serviceProvider
                .GetService<ISafeExecuteWrapper>()
                .ExecuteAsync(() => func.Invoke(c).ConfigureAwait(false), c);

            if (isBackground)
            {
                return;
            }

            await task;
        });
    }

    public ICommand CreateCommand<TValidationErrors>(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand(async ct =>
        {
            var c = canCancel ? ct : CancellationToken.None;
            var task = _serviceProvider
                .GetService<ISafeExecuteWrapper>()
                .ExecuteAsync(() => func.Invoke(c), c);

            if (isBackground)
            {
                return;
            }

            await task;
        });
    }

    public ICommand CreateCommand<TValidationErrors>(
        Func<CancellationToken, ValueTask<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand(async ct =>
        {
            var c = canCancel ? ct : CancellationToken.None;
            var task = _serviceProvider
                .GetService<ISafeExecuteWrapper>()
                .ExecuteAsync(() => func.Invoke(c).ConfigureAwait(false), c);

            if (isBackground)
            {
                return;
            }

            await task;
        });
    }

    public ICommand CreateCommand<T, TValidationErrors>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand<T>(
            async (parameter, ct) =>
            {
                var c = canCancel ? ct : CancellationToken.None;

                var task = _serviceProvider
                    .GetService<ISafeExecuteWrapper>()
                    .ExecuteAsync(() => func.Invoke(parameter.ThrowIfNull(), c), c);

                if (isBackground)
                {
                    return;
                }

                await task;
            }
        );
    }

    public ICommand CreateCommand<T, TValidationErrors>(
        Func<T, CancellationToken, ValueTask<TValidationErrors>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand<T>(
            async (parameter, ct) =>
            {
                var c = canCancel ? ct : CancellationToken.None;

                var task = _serviceProvider
                    .GetService<ISafeExecuteWrapper>()
                    .ExecuteAsync(
                        () => func.Invoke(parameter.ThrowIfNull(), c).ConfigureAwait(false),
                        c
                    );

                if (isBackground)
                {
                    return;
                }

                await task;
            }
        );
    }

    private readonly IServiceProvider _serviceProvider;
}
