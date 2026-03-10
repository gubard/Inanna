using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;
using Microsoft.Extensions.Logging;

namespace Inanna.Helpers;

public static class UiHelper
{
    static UiHelper()
    {
        Logger = DiHelper.ServiceProvider.GetService<ILogger>();
        ProgressService = DiHelper.ServiceProvider.GetService<IProgressService>();
        DialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
        Navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        AppResourceService = DiHelper.ServiceProvider.GetService<IAppResourceService>();
        ErrorDialogFactory = DiHelper.ServiceProvider.GetService<IErrorDialogFactory>();
        EmptyCommand = new RelayCommand(() => { });

        CancelButton = new(
            AppResourceService.GetResource<string>("Lang.Cancel"),
            new AsyncRelayCommand(async ct => await DialogService.CloseMessageBoxAsync(ct)),
            null,
            DialogButtonType.Normal
        );

        OkButton = new(
            AppResourceService.GetResource<string>("Lang.Ok"),
            new AsyncRelayCommand(async ct => await DialogService.CloseMessageBoxAsync(ct)),
            null,
            DialogButtonType.Primary
        );
    }

    public static readonly SelectionMode MultipleToggle =
        SelectionMode.Multiple | SelectionMode.Toggle;

    public static readonly DialogButton CancelButton;
    public static readonly DialogButton OkButton;
    public static readonly ICommand EmptyCommand;

    public static ConfiguredValueTaskAwaitable NavigateToAsync<TView>(CancellationToken ct)
        where TView : notnull
    {
        return Navigator.NavigateToAsync(DiHelper.ServiceProvider.GetService<TView>(), ct);
    }

    public static ConfiguredValueTaskAwaitable ExecuteAsync(
        Func<ConfiguredValueTaskAwaitable> func,
        CancellationToken ct
    )
    {
        return ExecuteCore(func, ct).ConfigureAwait(false);
    }

    public static ConfiguredValueTaskAwaitable ExecuteAsync<TValidationErrors>(
        Func<ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        return ExecuteCore(func, ct).ConfigureAwait(false);
    }

    public static void Execute<TValidationErrors>(Func<TValidationErrors> func)
        where TValidationErrors : IValidationErrors
    {
        var progress = new ProgressItem(1);

        try
        {
            ProgressService.AddProgress(progress);
            var result = func.Invoke();

            if (result.ValidationErrors is not { Count: 0 })
            {
                Logger.CommandErrors(result.ValidationErrors);

                DialogService.ShowMessageBoxAsync(
                    ErrorDialogFactory.Create(result.ValidationErrors.ToArray()),
                    CancellationToken.None
                );
            }
        }
        catch (Exception e)
        {
            Logger.CommandException(e);

            DialogService.ShowMessageBoxAsync(
                ErrorDialogFactory.Create([e]),
                CancellationToken.None
            );
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }

    public static void Execute(Action action)
    {
        var progress = new ProgressItem(1);

        try
        {
            ProgressService.AddProgress(progress);
            action.Invoke();
        }
        catch (Exception e)
        {
            Logger.CommandException(e);

            DialogService.ShowMessageBoxAsync(
                ErrorDialogFactory.Create([e]),
                CancellationToken.None
            );
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }

    private static async ValueTask ExecuteCore(
        Func<ConfiguredValueTaskAwaitable> func,
        CancellationToken ct
    )
    {
        var progress = new ProgressItem(1);

        try
        {
            ProgressService.AddProgress(progress);
            await func.Invoke();
        }
        catch (Exception e)
        {
            Logger.CommandException(e);
            await DialogService.ShowMessageBoxAsync(ErrorDialogFactory.Create([e]), ct);
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }

    public static ICommand CreateCommand<T>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable> func,
        bool isBackground = false
    )
    {
        return new AsyncRelayCommand<T>(
            async (parameter, ct) =>
            {
                var c = isBackground ? CancellationToken.None : ct;
                var task = ExecuteAsync(() => func.Invoke(parameter.ThrowIfNull(), c), c);

                if (isBackground)
                {
                    return;
                }

                await task;
            }
        );
    }

    public static ICommand CreateCommand(
        Func<CancellationToken, ConfiguredValueTaskAwaitable> func,
        bool isBackground = false
    )
    {
        return new AsyncRelayCommand(async ct =>
        {
            var c = isBackground ? CancellationToken.None : ct;
            var task = ExecuteAsync(() => func.Invoke(c), c);

            if (isBackground)
            {
                return;
            }

            await task;
        });
    }

    public static ICommand CreateCommand(
        Func<CancellationToken, ValueTask> func,
        bool isBackground = false
    )
    {
        return new AsyncRelayCommand(async ct =>
        {
            var c = isBackground ? CancellationToken.None : ct;
            var task = ExecuteAsync(() => func.Invoke(c).ConfigureAwait(false), c);

            if (isBackground)
            {
                return;
            }

            await task;
        });
    }

    public static ICommand CreateCommand<TValidationErrors>(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        bool isBackground = false
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand(async ct =>
        {
            var c = isBackground ? CancellationToken.None : ct;
            var task = ExecuteAsync(() => func.Invoke(c), c);

            if (isBackground)
            {
                return;
            }

            await task;
        });
    }

    public static ICommand CreateCommand<TValidationErrors>(
        Func<CancellationToken, ValueTask<TValidationErrors>> func,
        bool isBackground = false
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand(async ct =>
        {
            var c = isBackground ? CancellationToken.None : ct;
            var task = ExecuteAsync(() => func.Invoke(c).ConfigureAwait(false), c);

            if (isBackground)
            {
                return;
            }

            await task;
        });
    }

    public static ICommand CreateCommand<T, TValidationErrors>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        bool isBackground = false
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand<T>(
            async (parameter, ct) =>
            {
                var c = isBackground ? CancellationToken.None : ct;
                var task = ExecuteAsync(() => func.Invoke(parameter.ThrowIfNull(), c), c);

                if (isBackground)
                {
                    return;
                }

                await task;
            }
        );
    }

    public static ConfiguredValueTaskAwaitable<ChangeOrderParameters<T>?> ShowChangeOrderAsync<T>(
        T[] items,
        T[] changeOrderItems,
        CancellationToken ct
    )
        where T : class, IOrderedItem
    {
        return ShowChangeOrderCore(items, changeOrderItems, ct).ConfigureAwait(false);
    }

    private static readonly IDialogService DialogService;
    private static readonly INavigator Navigator;
    private static readonly IAppResourceService AppResourceService;
    private static readonly IProgressService ProgressService;
    private static readonly IErrorDialogFactory ErrorDialogFactory;
    private static readonly ILogger Logger;

    private static async ValueTask ExecuteCore<TValidationErrors>(
        Func<ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        var progress = new ProgressItem(1);

        try
        {
            ProgressService.AddProgress(progress);
            var result = await func.Invoke();

            if (result.ValidationErrors is not { Count: 0 })
            {
                Logger.CommandErrors(result.ValidationErrors);

                await DialogService.ShowMessageBoxAsync(
                    ErrorDialogFactory.Create(result.ValidationErrors.ToArray()),
                    ct
                );
            }
        }
        catch (Exception e)
        {
            Logger.CommandException(e);
            await DialogService.ShowMessageBoxAsync(ErrorDialogFactory.Create([e]), ct);
        }
        finally
        {
            Dispatcher.UIThread.Post(() => progress.CurrentValue++);
        }
    }

    private static async ValueTask<ChangeOrderParameters<T>?> ShowChangeOrderCore<T>(
        T[] items,
        T[] changeOrderItems,
        CancellationToken ct
    )
        where T : class, IOrderedItem
    {
        foreach (var item in items)
        {
            item.IsChangingOrder = changeOrderItems.Contains(item);
        }

        var viewModel = new ChangeOrderViewModel(items.OrderBy(x => x.OrderIndex));
        ChangeOrderParameters<T>? result = null;

        await DialogService.ShowMessageBoxAsync(
            new(
                AppResourceService.GetResource<string>("Lang.ChangeOrder").DispatchToDialogHeader(),
                viewModel,
                new(
                    AppResourceService.GetResource<string>("Lang.Ok"),
                    new AsyncRelayCommand(async ct =>
                    {
                        result = new(viewModel.SelectedItem.Cast<T>(), viewModel.IsAfter);
                        await DialogService.CloseMessageBoxAsync(ct);
                    }),
                    null,
                    DialogButtonType.Primary
                ),
                CancelButton
            ),
            ct
        );

        return result;
    }
}
