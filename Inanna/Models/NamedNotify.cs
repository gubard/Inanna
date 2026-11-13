using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public sealed partial class NamedNotify : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    [ObservableProperty]
    public partial object? Value { get; set; }
}
