using System.Runtime.CompilerServices;
using System.Windows.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;

namespace Inanna.Helpers;

public static class UiHelper
{
    static UiHelper()
    {
        DialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
        Navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        AppResourceService = DiHelper.ServiceProvider.GetService<IAppResourceService>();
        EmptyCommand = new RelayCommand(() => { });

        CancelButton = new(
            AppResourceService.GetResource<string>("Lang.Cancel"),
            new RelayCommand(() => DialogService.DispatchCloseMessageBox()),
            null,
            DialogButtonType.Normal
        );

        OkButton = new(
            AppResourceService.GetResource<string>("Lang.Ok"),
            new RelayCommand(() => DialogService.DispatchCloseMessageBox()),
            null,
            DialogButtonType.Primary
        );
    }

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
        try
        {
            var result = func.Invoke();

            if (result.ValidationErrors is not { Count: 0 })
            {
                DialogService.ShowMessageBoxAsync(
                    new(
                        Dispatcher.UIThread.Invoke(() =>
                            AppResourceService.GetResource<string>("Lang.Error").ToDialogHeader()
                        ),
                        new ValidationErrorsViewModel(result.ValidationErrors.ToArray()),
                        OkButton
                    ),
                    CancellationToken.None
                );
            }
        }
        catch (Exception e)
        {
            DialogService.ShowMessageBoxAsync(
                new(
                    Dispatcher.UIThread.Invoke(() =>
                        AppResourceService.GetResource<string>("Lang.Error").ToDialogHeader()
                    ),
                    new ExceptionViewModel(e),
                    OkButton
                ),
                CancellationToken.None
            );
        }
    }

    public static void Execute(Action action)
    {
        try
        {
            action.Invoke();
        }
        catch (Exception e)
        {
            DialogService.ShowMessageBoxAsync(
                new(
                    Dispatcher.UIThread.Invoke(() =>
                        AppResourceService.GetResource<string>("Lang.Error").ToDialogHeader()
                    ),
                    new ExceptionViewModel(e),
                    OkButton
                ),
                CancellationToken.None
            );
        }
    }

    public static ConfiguredValueTaskAwaitable<bool> CheckValidationErrorsAsync<TValidationErrors>(
        ConfiguredValueTaskAwaitable<TValidationErrors> task,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        return CheckValidationErrorsCore(task, ct).ConfigureAwait(false);
    }

    private static async ValueTask ExecuteCore(
        Func<ConfiguredValueTaskAwaitable> func,
        CancellationToken ct
    )
    {
        try
        {
            await func.Invoke();
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageBoxAsync(
                new(
                    Dispatcher.UIThread.Invoke(() =>
                        AppResourceService.GetResource<string>("Lang.Error").ToDialogHeader()
                    ),
                    new ExceptionViewModel(e),
                    OkButton
                ),
                ct
            );
        }
    }

    public static ConfiguredValueTaskAwaitable<bool> CheckValidationErrorsAsync<TValidationErrors>(
        TValidationErrors errors,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        return CheckValidationErrorsCore(errors, ct).ConfigureAwait(false);
    }

    public static ICommand CreateCommand<T>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable> func
    )
    {
        return new AsyncRelayCommand<T>(
            async (parameter, ct) =>
                await ExecuteAsync(() => func.Invoke(parameter.ThrowIfNull(), ct), ct)
        );
    }

    public static ICommand CreateCommand(Func<CancellationToken, ConfiguredValueTaskAwaitable> func)
    {
        return new AsyncRelayCommand(async ct => await ExecuteAsync(() => func.Invoke(ct), ct));
    }

    public static ICommand CreateCommand<TValidationErrors>(
        Func<CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand(async ct => await ExecuteAsync(() => func.Invoke(ct), ct));
    }

    public static ICommand CreateCommand<T, TValidationErrors>(
        Func<T, CancellationToken, ConfiguredValueTaskAwaitable<TValidationErrors>> func
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand<T>(
            async (parameter, ct) =>
                await ExecuteAsync(() => func.Invoke(parameter.ThrowIfNull(), ct), ct)
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

    private static async ValueTask ExecuteCore<TValidationErrors>(
        Func<ConfiguredValueTaskAwaitable<TValidationErrors>> func,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        try
        {
            var result = await func.Invoke();

            if (result.ValidationErrors is not { Count: 0 })
            {
                await DialogService.ShowMessageBoxAsync(
                    new(
                        Dispatcher.UIThread.Invoke(() =>
                            AppResourceService.GetResource<string>("Lang.Error").ToDialogHeader()
                        ),
                        new ValidationErrorsViewModel(result.ValidationErrors.ToArray()),
                        OkButton
                    ),
                    ct
                );
            }
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageBoxAsync(
                new(
                    Dispatcher.UIThread.Invoke(() =>
                        AppResourceService.GetResource<string>("Lang.Error").ToDialogHeader()
                    ),
                    new ExceptionViewModel(e),
                    OkButton
                ),
                ct
            );
        }
    }

    private static async ValueTask<bool> CheckValidationErrorsCore<TValidationErrors>(
        ConfiguredValueTaskAwaitable<TValidationErrors> task,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        var result = await task;

        if (result.ValidationErrors is { Count: 0 })
        {
            return true;
        }

        await DialogService.ShowMessageBoxAsync(
            new(
                Dispatcher.UIThread.Invoke(() =>
                    AppResourceService.GetResource<string>("Lang.Error").ToDialogHeader()
                ),
                new ValidationErrorsViewModel(result.ValidationErrors.ToArray()),
                OkButton
            ),
            ct
        );

        return false;
    }

    private static async ValueTask<bool> CheckValidationErrorsCore<TValidationErrors>(
        TValidationErrors errors,
        CancellationToken ct
    )
        where TValidationErrors : IValidationErrors
    {
        if (errors.ValidationErrors is { Count: 0 })
        {
            return true;
        }

        await DialogService.ShowMessageBoxAsync(
            new(
                Dispatcher.UIThread.Invoke(() =>
                    AppResourceService.GetResource<string>("Lang.Error").ToDialogHeader()
                ),
                new ValidationErrorsViewModel(errors.ValidationErrors.ToArray()),
                OkButton
            ),
            ct
        );

        return false;
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

        var viewModel = Dispatcher.UIThread.Invoke(() =>
            new ChangeOrderViewModel(items.OrderBy(x => x.OrderIndex))
        );

        ChangeOrderParameters<T>? result = null;

        await DialogService.ShowMessageBoxAsync(
            new(
                Dispatcher.UIThread.Invoke(() =>
                    AppResourceService.GetResource<string>("Lang.ChangeOrder").ToDialogHeader()
                ),
                viewModel,
                new(
                    AppResourceService.GetResource<string>("Lang.Ok"),
                    new RelayCommand(() =>
                    {
                        result = new(viewModel.SelectedItem.Cast<T>(), viewModel.IsAfter);
                        DialogService.DispatchCloseMessageBox();
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
