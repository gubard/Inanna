using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public sealed partial class ProgressItem : ObservableObject
{
    public ProgressItem(uint needValue)
    {
        NeedValue = needValue;
    }

    public uint NeedValue { get; }

    [ObservableProperty]
    public partial string Status { get; set; } = string.Empty;

    [ObservableProperty]
    public partial uint CurrentValue { get; set; }
}
