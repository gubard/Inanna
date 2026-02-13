using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public sealed partial class UserState : ObservableObject
{
    [ObservableProperty]
    private string _login = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private Guid _id;
}
