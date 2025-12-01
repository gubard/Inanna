using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;

namespace Inanna.Helpers;

public static class UiHelper
{
    private static readonly IDialogService DialogService;
    private static readonly INavigator Navigator;
    private static readonly IApplicationResourceService ApplicationResourceService;

    static UiHelper()
    {
        DialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
        Navigator = DiHelper.ServiceProvider.GetService<INavigator>();
        ApplicationResourceService = DiHelper.ServiceProvider.GetService<IApplicationResourceService>();
        EmptyCommand = new RelayCommand(() => { });

        CancelButton = new(ApplicationResourceService.GetResource<string>("Lang.Cancel"),
            new RelayCommand(() => DialogService.CloseMessageBox()), null, DialogButtonType.Normal);

        OkButton = new(ApplicationResourceService.GetResource<string>("Lang.Ok"),
            new RelayCommand(() => DialogService.CloseMessageBox()), null, DialogButtonType.Primary);
    }

    public static readonly DialogButton CancelButton;
    public static readonly DialogButton OkButton;
    public static readonly ICommand EmptyCommand;

    public static Task NavigateToAsync<TView>(CancellationToken ct) where TView : notnull
    {
        return Navigator.NavigateToAsync(DiHelper.ServiceProvider.GetService<TView>(), ct);
    }

    public static async Task ExecuteAsync(Func<Task> func)
    {
        try
        {
            await func.Invoke();
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageBoxAsync(new(ApplicationResourceService.GetResource<string>("Lang.Error"),
                new ExceptionViewModel(e), OkButton));
        }
    }

    public static async Task ExecuteAsync(Func<Task<IValidationErrors>> func)
    {
        try
        {
            var result = await func.Invoke();

            if (result.ValidationErrors is not { Length: 0 })
            {
                await DialogService.ShowMessageBoxAsync(new(ApplicationResourceService.GetResource<string>("Lang.Error"),
                    new ValidationErrorsViewModel(result.ValidationErrors), OkButton));
            }
        }
        catch (Exception e)
        {
            await DialogService.ShowMessageBoxAsync(new(ApplicationResourceService.GetResource<string>("Lang.Error"),
                new ExceptionViewModel(e), OkButton));
        }
    }

    public static async ValueTask<bool> CheckValidationErrors<TValidationErrors>(ValueTask<TValidationErrors> func) where TValidationErrors : IValidationErrors
    {
        var result = await func;

        if (result.ValidationErrors is { Length: 0 })
        {
            return true;
        }

        await DialogService.ShowMessageBoxAsync(new(ApplicationResourceService.GetResource<string>("Lang.Error"),
            new ValidationErrorsViewModel(result.ValidationErrors), OkButton));

        return false;
    }

    public static ICommand CreateCommand(Func<Task> func)
    {
        return new AsyncRelayCommand(() => ExecuteAsync(func));
    }

    public static ICommand CreateCommand(Func<Task<IValidationErrors>> func)
    {
        return new AsyncRelayCommand(() => ExecuteAsync(func));
    }
}