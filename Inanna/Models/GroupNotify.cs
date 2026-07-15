using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public sealed partial class GroupNotify<T> : ObservableObject
{
    public AvaloniaList<T> Items { get; } = new();

    [ObservableProperty]
    private string _name = string.Empty;
}
