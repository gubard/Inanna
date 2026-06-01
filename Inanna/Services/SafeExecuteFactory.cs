using System.Runtime.CompilerServices;
using Avalonia.Threading;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Microsoft.Extensions.Logging;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Inanna.Services;

public interface ISafeExecuteWrapper
{
    void Execute(Action action);

    void Execute<TValidationErrors>(Func<TValidationErrors> func)
        where TValidationErrors : IValidationErrors;

    public ConfiguredValueTaskAwaitable ExecuteAsync(
        Func<ConfiguredValueTaskAwaitable> func,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable ExecuteAsync<TValidationErrors>(
        Func<ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors;
}

public sealed class SafeExecuteWrapper : ISafeExecuteWrapper
{
    public SafeExecuteWrapper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ConfiguredValueTaskAwaitable ExecuteAsync(
        Func<ConfiguredValueTaskAwaitable> func,
        CancellationToken ct
    )
    {
        return ExecuteCore(func, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable ExecuteAsync<TValidationErrors>(
        Func<ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        return ExecuteCore(func, ct).ConfigureAwait(false);
    }

    public void Execute<TValidationErrors>(Func<TValidationErrors> func)
        where TValidationErrors : IValidationErrors
    {
        var progress = new ProgressItem(1);

        try
        {
            _serviceProvider.GetService<IProgressService>().AddProgress(progress);
            var result = func.Invoke();

            if (result.ValidationErrors is not { Count: 0 })
            {
                _serviceProvider.GetService<ILogger>().CommandErrors(result.ValidationErrors);

                _serviceProvider
                    .GetService<IDialogService>()
                    .ShowMessageBoxAsync(
                        _serviceProvider
                            .GetService<IErrorDialogFactory>()
                            .Create(result.ValidationErrors.ToArray()),
                        CancellationToken.None
                    );
            }
        }
        catch (OperationCanceledException e)
        {
            _serviceProvider.GetService<ILogger>().OperationCanceled(e);
        }
        catch (Exception e)
        {
            _serviceProvider.GetService<ILogger>().CommandException(e);

            _serviceProvider
                .GetService<IDialogService>()
                .ShowMessageBoxAsync(
                    _serviceProvider.GetService<IErrorDialogFactory>().Create([e]),
                    CancellationToken.None
                );
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }

    public void Execute(Action action)
    {
        var progress = new ProgressItem(1);

        try
        {
            _serviceProvider.GetService<IProgressService>().AddProgress(progress);
            action.Invoke();
        }
        catch (OperationCanceledException e)
        {
            _serviceProvider.GetService<ILogger>().OperationCanceled(e);
        }
        catch (Exception e)
        {
            _serviceProvider.GetService<ILogger>().CommandException(e);

            _serviceProvider
                .GetService<IDialogService>()
                .ShowMessageBoxAsync(
                    _serviceProvider.GetService<IErrorDialogFactory>().Create([e]),
                    CancellationToken.None
                );
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }

    private readonly IServiceProvider _serviceProvider;

    private async ValueTask ExecuteCore(
        Func<ConfiguredValueTaskAwaitable> func,
        CancellationToken ct
    )
    {
        var progress = new ProgressItem(1);

        try
        {
            _serviceProvider.GetService<IProgressService>().AddProgress(progress);
            await func.Invoke();
        }
        catch (OperationCanceledException e)
        {
            _serviceProvider.GetService<ILogger>().OperationCanceled(e);
        }
        catch (Exception e)
        {
            _serviceProvider.GetService<ILogger>().CommandException(e);
            await _serviceProvider
                .GetService<IDialogService>()
                .ShowMessageBoxAsync(
                    _serviceProvider.GetService<IErrorDialogFactory>().Create([e]),
                    ct
                );
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }

    private async ValueTask ExecuteCore<TValidationErrors>(
        Func<ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        var progress = new ProgressItem(1);

        try
        {
            _serviceProvider.GetService<IProgressService>().AddProgress(progress);
            var result = await func.Invoke();

            if (result.ValidationErrors is not { Count: 0 })
            {
                _serviceProvider.GetService<ILogger>().CommandErrors(result.ValidationErrors);

                await _serviceProvider
                    .GetService<IDialogService>()
                    .ShowMessageBoxAsync(
                        _serviceProvider
                            .GetService<IErrorDialogFactory>()
                            .Create(result.ValidationErrors.ToArray()),
                        ct
                    );
            }
        }
        catch (OperationCanceledException e)
        {
            _serviceProvider.GetService<ILogger>().OperationCanceled(e);
        }
        catch (Exception e)
        {
            _serviceProvider.GetService<ILogger>().CommandException(e);
            await _serviceProvider
                .GetService<IDialogService>()
                .ShowMessageBoxAsync(
                    _serviceProvider.GetService<IErrorDialogFactory>().Create([e]),
                    ct
                );
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }
}
