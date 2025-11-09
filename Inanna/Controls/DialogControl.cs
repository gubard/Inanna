using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace Inanna.Controls;

public class DialogControl : ContentControl
{
    private static readonly Dictionary<string, DialogControl> Dialogs = new();

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<DialogControl, bool>(nameof(IsOpen), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<object?> DialogProperty =
        AvaloniaProperty.Register<DialogControl, object?>(nameof(Dialog), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> WidthDialogProperty =
        AvaloniaProperty.Register<DialogControl, double>(nameof(WidthDialog), double.NaN);

    public static readonly StyledProperty<double> HeightDialogProperty =
        AvaloniaProperty.Register<DialogControl, double>(nameof(HeightDialog), double.NaN);

    public static readonly StyledProperty<double> MaxWidthDialogProperty =
        AvaloniaProperty.Register<DialogControl, double>(nameof(MaxWidthDialog), double.PositiveInfinity);

    public static readonly StyledProperty<double> MaxHeightDialogProperty =
        AvaloniaProperty.Register<DialogControl, double>(nameof(MaxHeightDialog), double.PositiveInfinity);

    public static readonly StyledProperty<string?> IdentifierProperty =
        AvaloniaProperty.Register<DialogControl, string?>(nameof(Identifier));

    static DialogControl()
    {
        IsOpenProperty.Changed.AddClassHandler<DialogControl>((control, _) =>
            control.PseudoClasses.Set(":is-open", control.IsOpen));

        IdentifierProperty.Changed.AddClassHandler<DialogControl, string?>((control, id) =>
        {
            if (id.NewValue.Value is null)
            {
                return;
            }

            Dialogs[id.NewValue.Value] = control;
        });
    }
    
    public static bool IsShowDialog(string identifier)
    {
        return Dialogs[identifier].IsOpen;
    }

    public static void ShowDialog(string identifier, object dialog)
    {
        Dialogs[identifier].Dialog = dialog;
        Dialogs[identifier].IsOpen = true;
    }
    
    public static void CloseDialog(string identifier)
    {
        Dialogs[identifier].IsOpen = false;
    }

    public string? Identifier
    {
        get => GetValue(IdentifierProperty);
        set => SetValue(IdentifierProperty, value);
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public object? Dialog
    {
        get => GetValue(DialogProperty);
        set => SetValue(DialogProperty, value);
    }

    public double WidthDialog
    {
        get => GetValue(WidthDialogProperty);
        set => SetValue(WidthDialogProperty, value);
    }

    public double HeightDialog
    {
        get => GetValue(MaxWidthDialogProperty);
        set => SetValue(MaxWidthDialogProperty, value);
    }

    public double MaxWidthDialog
    {
        get => GetValue(WidthDialogProperty);
        set => SetValue(WidthDialogProperty, value);
    }

    public double MaxHeightDialog
    {
        get => GetValue(MaxHeightDialogProperty);
        set => SetValue(MaxHeightDialogProperty, value);
    }
}