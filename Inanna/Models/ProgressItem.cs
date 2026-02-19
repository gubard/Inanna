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
    private uint _currentValue;
}
