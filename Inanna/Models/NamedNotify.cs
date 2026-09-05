using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public sealed partial class NamedNotify : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private object? _value;
}
