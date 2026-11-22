using System.Runtime.CompilerServices;
using Avalonia.Threading;
using Gaia.Helpers;
using Gaia.Models;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Microsoft.Extensions.Logging;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Inanna.Services;

public interface ISafeExecuteWrapper
{
    void Execute(Action action);

    void Execute(Func<Result> func);

    public ConfiguredValueTaskAwaitable ExecuteAsync(
        Func<ConfiguredValueTaskAwaitable> func,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable ExecuteAsync(
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    );

    ConfiguredValueTaskAwaitable ExecuteAsync<T>(
        Func<ConfiguredValueTaskAwaitable<Result<T>>> func,
        CancellationToken ct
    );
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

    public ConfiguredValueTaskAwaitable ExecuteAsync(
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
    {
        return ExecuteCore(func, ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable ExecuteAsync<T>(
        Func<ConfiguredValueTaskAwaitable<Result<T>>> func,
        CancellationToken ct
    )
    {
        return ExecuteCore(func, ct).ConfigureAwait(false);
    }

    public void Execute(Func<Result> func)
    {
        var progress = new ProgressItem(1);

        try
        {
            _serviceProvider.GetService<IProgressService>().AddProgress(progress);
            var result = func.Invoke();

            if (result is ValidationError error)
            {
                _serviceProvider.GetService<ILogger>().CommandErrors(error);

                _serviceProvider
                    .GetService<IDialogService>()
                    .ShowMessageBoxAsync(
                        _serviceProvider
                            .GetService<IAppResourceService>()
                            .GetResource<string>("Lang.Error")
                            .DispatchToDialogHeader(),
                        _serviceProvider.GetService<IErrorDialogFactory>().Create(error),
                        CancellationToken.None,
                        _serviceProvider.GetService<IDialogService>().OkButton
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
                    _serviceProvider
                        .GetService<IAppResourceService>()
                        .GetResource<string>("Lang.Error")
                        .DispatchToDialogHeader(),
                    _serviceProvider.GetService<IErrorDialogFactory>().Create([e]),
                    CancellationToken.None,
                    _serviceProvider.GetService<IDialogService>().OkButton
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
                    _serviceProvider
                        .GetService<IAppResourceService>()
                        .GetResource<string>("Lang.Error")
                        .DispatchToDialogHeader(),
                    _serviceProvider.GetService<IErrorDialogFactory>().Create([e]),
                    CancellationToken.None,
                    _serviceProvider.GetService<IDialogService>().OkButton
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
                    _serviceProvider
                        .GetService<IAppResourceService>()
                        .GetResource<string>("Lang.Error")
                        .DispatchToDialogHeader(),
                    _serviceProvider.GetService<IErrorDialogFactory>().Create([e]),
                    CancellationToken.None,
                    _serviceProvider.GetService<IDialogService>().OkButton
                );
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }

    private async ValueTask ExecuteCore(
        Func<ConfiguredValueTaskAwaitable<Result>> func,
        CancellationToken ct
    )
    {
        var progress = new ProgressItem(1);

        try
        {
            _serviceProvider.GetService<IProgressService>().AddProgress(progress);
            var result = await func.Invoke();

            if (result is ValidationError error)
            {
                _serviceProvider.GetService<ILogger>().CommandErrors(error);

                await _serviceProvider
                    .GetService<IDialogService>()
                    .ShowMessageBoxAsync(
                        _serviceProvider
                            .GetService<IAppResourceService>()
                            .GetResource<string>("Lang.Error")
                            .DispatchToDialogHeader(),
                        _serviceProvider.GetService<IErrorDialogFactory>().Create(error),
                        CancellationToken.None,
                        _serviceProvider.GetService<IDialogService>().OkButton
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
                    _serviceProvider
                        .GetService<IAppResourceService>()
                        .GetResource<string>("Lang.Error")
                        .DispatchToDialogHeader(),
                    _serviceProvider.GetService<IErrorDialogFactory>().Create([e]),
                    CancellationToken.None,
                    _serviceProvider.GetService<IDialogService>().OkButton
                );
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }

    private async ValueTask ExecuteCore<T>(
        Func<ConfiguredValueTaskAwaitable<Result<T>>> func,
        CancellationToken ct
    )
    {
        var progress = new ProgressItem(1);

        try
        {
            _serviceProvider.GetService<IProgressService>().AddProgress(progress);
            var result = await func.Invoke();

            if (result is ValidationError error)
            {
                _serviceProvider.GetService<ILogger>().CommandErrors(error);

                await _serviceProvider
                    .GetService<IDialogService>()
                    .ShowMessageBoxAsync(
                        _serviceProvider
                            .GetService<IAppResourceService>()
                            .GetResource<string>("Lang.Error")
                            .DispatchToDialogHeader(),
                        _serviceProvider.GetService<IErrorDialogFactory>().Create(error),
                        CancellationToken.None,
                        _serviceProvider.GetService<IDialogService>().OkButton
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
                    _serviceProvider
                        .GetService<IAppResourceService>()
                        .GetResource<string>("Lang.Error")
                        .DispatchToDialogHeader(),
                    _serviceProvider.GetService<IErrorDialogFactory>().Create([e]),
                    CancellationToken.None,
                    _serviceProvider.GetService<IDialogService>().OkButton
                );
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }
}
