using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

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
        IsPasswordShowProperty.Changed.AddClassHandler<Button, bool>((button, e) =>
        {
            if (e.NewValue.GetValueOrDefault<bool>())
            {
                button.Click += OnClick;
                button.SetPseudoClass(":show-password", true);
                button.SetPseudoClass(":hide-password", false);
                var passwordTextBox = GetPasswordTextBox(button);

                if (passwordTextBox is null)
                {
                    return;
                }

                passwordTextBox.SetPseudoClass(":show-password", true);
                passwordTextBox.SetPseudoClass(":hide-password", false);
            }
            else
            {
                button.Click -= OnClick;
                button.SetPseudoClass(":show-password", false);
                button.SetPseudoClass(":hide-password", false);

                var passwordTextBox = GetPasswordTextBox(button);

                if (passwordTextBox is null)
                {
                    return;
                }

                passwordTextBox.SetPseudoClass(":show-password", false);
                passwordTextBox.SetPseudoClass(":hide-password", false);
            }
        });
    }

    private static TextBox? GetPasswordTextBox(Button button)
    {
        return button.GetVisualAncestors().OfType<TextBox>().FirstOrDefault();
    }

    private static void OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var passwordTextBox = GetPasswordTextBox(button);

        if (passwordTextBox is null)
        {
            return;
        }

        switch (passwordTextBox.PasswordChar)
        {
            case '\0':
            {
                button.SetPseudoClass(":show-password", true);
                button.SetPseudoClass(":hide-password", false);
                passwordTextBox.SetPseudoClass(":show-password", true);
                passwordTextBox.SetPseudoClass(":hide-password", false);

                break;
            }
            default:
            {
                button.SetPseudoClass(":show-password", false);
                button.SetPseudoClass(":hide-password", true);
                passwordTextBox.SetPseudoClass(":show-password", false);
                passwordTextBox.SetPseudoClass(":hide-password", true);

                break;
            }
        }
    }
}