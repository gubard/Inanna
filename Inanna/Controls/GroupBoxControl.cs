using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace Inanna.Controls;

public class GroupBoxControl : ContentControl
{
    public static readonly StyledProperty<object?> HeaderProperty = AvaloniaProperty.Register<
        GroupBoxControl,
        object?
    >(nameof(Header), defaultBindingMode: BindingMode.TwoWay);

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }
}
