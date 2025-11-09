using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Inanna.Models;
using Inanna.Services;
using Inanna.Ui;

namespace Inanna.Helpers;

public static class UiHelper
{
    private static readonly IDialogService DialogService;
    private static readonly IApplicationResourceService ApplicationResourceService;

    static UiHelper()
    {
        DialogService = DiHelper.ServiceProvider.GetService<IDialogService>();
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

    public static ICommand CreateCommand(Func<Task> func)
    {
        return new AsyncRelayCommand(() => ExecuteAsync(func));
    }
}