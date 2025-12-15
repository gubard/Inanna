using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;

namespace Inanna.Helpers;

public static class UiHelper
{
    private static readonly IDialogService DialogService;
    private static readonly INavigator Navigator;
    private static readonly IAppResourceService AppResourceService;

    static UiHelper()
    {
        DialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
        Navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        AppResourceService = DiHelper.ServiceProvider.GetService<IAppResourceService>();
        EmptyCommand = new RelayCommand(() => { });

        CancelButton = new(
            AppResourceService.GetResource<string>("Lang.Cancel"),
            new RelayCommand(() => DialogService.CloseMessageBox()),
            null,
            DialogButtonType.Normal
        );

        OkButton = new(
            AppResourceService.GetResource<string>("Lang.Ok"),
            new RelayCommand(() => DialogService.CloseMessageBox()),
            null,
            DialogButtonType.Primary
        );
    }

    public static readonly DialogButton CancelButton;
    public static readonly DialogButton OkButton;
    public static readonly ICommand EmptyCommand;

    public static ValueTask NavigateToAsync<TView>(CancellationToken ct)
        where TView : notnull
    {
        return Navigator.NavigateToAsync(DiHelper.ServiceProvider.GetService<TView>(), ct);
    }

    public static async ValueTask ExecuteAsync(Func<ValueTask> func)
    {
        try
        {
            await func.Invoke();
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageBoxAsync(
                new(
                    AppResourceService.GetResource<string>("Lang.Error"),
                    new ExceptionViewModel(e),
                    OkButton
                )
            );
        }
    }

    public static async ValueTask ExecuteAsync<TValidationErrors>(
        Func<ValueTask<TValidationErrors>> func
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
                        AppResourceService.GetResource<string>("Lang.Error"),
                        new ValidationErrorsViewModel(result.ValidationErrors.ToArray()),
                        OkButton
                    )
                );
            }
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageBoxAsync(
                new(
                    AppResourceService.GetResource<string>("Lang.Error"),
                    new ExceptionViewModel(e),
                    OkButton
                )
            );
        }
    }

    public static void Execute<TValidationErrors>(Func<TValidationErrors> func)
        where TValidationErrors : IValidationErrors
    {
        try
        {
            var result = func.Invoke();

            if (result.ValidationErrors is not { Count: 0 })
            {
                DialogService.ShowMessageBox(
                    new(
                        AppResourceService.GetResource<string>("Lang.Error"),
                        new ValidationErrorsViewModel(result.ValidationErrors.ToArray()),
                        OkButton
                    )
                );
            }
        }
        catch (Exception e)
        {
            DialogService.ShowMessageBox(
                new(
                    AppResourceService.GetResource<string>("Lang.Error"),
                    new ExceptionViewModel(e),
                    OkButton
                )
            );
        }
    }

    public static async ValueTask<bool> CheckValidationErrorsAsync<TValidationErrors>(
        ValueTask<TValidationErrors> task
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
                AppResourceService.GetResource<string>("Lang.Error"),
                new ValidationErrorsViewModel(result.ValidationErrors.ToArray()),
                OkButton
            )
        );

        return false;
    }

    public static async ValueTask<bool> CheckValidationErrorsAsync<TValidationErrors>(
        TValidationErrors errors
    )
        where TValidationErrors : IValidationErrors
    {
        if (errors.ValidationErrors is { Count: 0 })
        {
            return true;
        }

        await DialogService.ShowMessageBoxAsync(
            new(
                AppResourceService.GetResource<string>("Lang.Error"),
                new ValidationErrorsViewModel(errors.ValidationErrors.ToArray()),
                OkButton
            )
        );

        return false;
    }

    public static ICommand CreateCommand<T>(Func<T, CancellationToken, ValueTask> func)
    {
        return new AsyncRelayCommand<T>(
            async (parameter, ct) =>
                await ExecuteAsync(() => func.Invoke(parameter.ThrowIfNull(), ct))
        );
    }

    public static ICommand CreateCommand(Func<CancellationToken, ValueTask> func)
    {
        return new AsyncRelayCommand(async ct => await ExecuteAsync(() => func.Invoke(ct)));
    }

    public static ICommand CreateCommand<TValidationErrors>(
        Func<CancellationToken, ValueTask<TValidationErrors>> func
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand(async ct => await ExecuteAsync(() => func.Invoke(ct)));
    }

    public static ICommand CreateCommand<T, TValidationErrors>(
        Func<T, CancellationToken, ValueTask<TValidationErrors>> func
    )
        where TValidationErrors : IValidationErrors
    {
        return new AsyncRelayCommand<T>(
            async (parameter, ct) =>
                await ExecuteAsync(() => func.Invoke(parameter.ThrowIfNull(), ct))
        );
    }
}
