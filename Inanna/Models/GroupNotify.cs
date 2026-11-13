using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public sealed partial class GroupNotify<T> : ObservableObject
{
    public AvaloniaList<T> Items { get; } = new();

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;
}
