using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using IconPacks.Avalonia.MaterialDesign;

namespace Inanna.Helpers;

public static class ButtonAssist
{
    public static readonly AttachedProperty<bool> IsPasswordShowProperty =
        AvaloniaProperty.RegisterAttached<Button, bool>("IsPasswordShow", typeof(ButtonAssist));

    public static void SetIsDragHandle(Button element, bool value)
    {
        element.SetValue(IsPasswordShowProperty, value);
    }

    public static bool GetIsDragHandle(Button element)
    {
        return element.GetValue(IsPasswordShowProperty);
    }

    static ButtonAssist()
    {
        IsPasswordShowProperty.Changed.AddClassHandler<Button, bool>((s, e) =>
        {
            if (e.NewValue.GetValueOrDefault<bool>())
            {
                s.Click += OnClick;
            }
            else
            {
                s.Click -= OnClick;
            }
        });
    }


    private static void OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var textBox = button.GetVisualAncestors().OfType<TextBox>().FirstOrDefault();

        if (textBox is null)
        {
            return;
        }

        switch (textBox.PasswordChar)
        {
            case '\0':
            {
                textBox.PasswordChar = '*';

                button.Content = new PackIconMaterialDesign
                {
                    Kind = PackIconMaterialDesignKind.Visibility,
                };

                break;
            }
            default:
            {
                textBox.PasswordChar = '\0';

                button.Content = new PackIconMaterialDesign
                {
                    Kind = PackIconMaterialDesignKind.VisibilityOff,
                };

                break;
            }
        }
    }
}