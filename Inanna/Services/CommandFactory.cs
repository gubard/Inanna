using System.Runtime.CompilerServices;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Models;
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

    ICommand CreateCommand(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
        bool isBackground = false,
        bool canCancel = true
    );

    ICommand CreateCommand(
        Func<CancellationToken, ValueTask<Result>> func,
        bool isBackground = false,
        bool canCancel = true
    );

    ICommand CreateCommand<T>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable<Result<T>>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where T : notnull;

    ICommand CreateCommand<T>(
        Func<T, CancellationToken, ValueTask<Result<T>>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where T : notnull;
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

    public ICommand CreateCommand(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<Result>> func,
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
        Func<CancellationToken, ValueTask<Result>> func,
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

    public ICommand CreateCommand<T>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable<Result<T>>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where T : notnull
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
        Func<T, CancellationToken, ValueTask<Result<T>>> func,
        bool isBackground = false,
        bool canCancel = true
    )
        where T : notnull
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
