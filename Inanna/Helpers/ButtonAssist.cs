using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Inanna.Helpers;

public static class ButtonAssist
{
    public static readonly AttachedProperty<bool> IsPasswordHandlerProperty =
        AvaloniaProperty.RegisterAttached<Button, bool>("IsPasswordHandler", typeof(ButtonAssist));

    public static readonly AttachedProperty<bool> IsClearAllHandlerProperty =
        AvaloniaProperty.RegisterAttached<Button, bool>("IsClearAllHandler", typeof(ButtonAssist));

    public static readonly AttachedProperty<bool> IsSelectAllHandlerProperty =
        AvaloniaProperty.RegisterAttached<Button, bool>("IsSelectAllHandler", typeof(ButtonAssist));

    public static void SetIsSelectAllHandler(Button element, bool value)
    {
        element.SetValue(IsSelectAllHandlerProperty, value);
    }

    public static bool GetIsSelectAllHandler(Button element)
    {
        return element.GetValue(IsSelectAllHandlerProperty);
    }

    public static void SetIsClearAllHandler(Button element, bool value)
    {
        element.SetValue(IsClearAllHandlerProperty, value);
    }

    public static bool GetIsClearAllHandler(Button element)
    {
        return element.GetValue(IsClearAllHandlerProperty);
    }

    public static void SetIsPasswordHandler(Button element, bool value)
    {
        element.SetValue(IsPasswordHandlerProperty, value);
    }

    public static bool GetIsPasswordHandler(Button element)
    {
        return element.GetValue(IsPasswordHandlerProperty);
    }

    static ButtonAssist()
    {
        IsSelectAllHandlerProperty.Changed.AddClassHandler<Button, bool>(
            (button, e) =>
            {
                if (e.NewValue.GetValueOrDefault<bool>())
                {
                    button.Click += OnSelectAllClick;
                }
                else
                {
                    button.Click -= OnSelectAllClick;
                }
            }
        );

        IsClearAllHandlerProperty.Changed.AddClassHandler<Button, bool>(
            (button, e) =>
            {
                if (e.NewValue.GetValueOrDefault<bool>())
                {
                    button.Click += OnClearAllClick;
                }
                else
                {
                    button.Click -= OnClearAllClick;
                }
            }
        );

        IsPasswordHandlerProperty.Changed.AddClassHandler<Button, bool>(
            (button, e) =>
            {
                if (e.NewValue.GetValueOrDefault<bool>())
                {
                    button.Click += OnPasswordClick;
                    button.SetPseudoClass(":show-password", true);
                    button.SetPseudoClass(":hide-password", false);
                    var passwordTextBox = GetParentTextBox(button);

                    if (passwordTextBox is null)
                    {
                        return;
                    }

                    passwordTextBox.SetPseudoClass(":show-password", true);
                    passwordTextBox.SetPseudoClass(":hide-password", false);
                }
                else
                {
                    button.Click -= OnPasswordClick;
                    button.SetPseudoClass(":show-password", false);
                    button.SetPseudoClass(":hide-password", false);

                    var passwordTextBox = GetParentTextBox(button);

                    if (passwordTextBox is null)
                    {
                        return;
                    }

                    passwordTextBox.SetPseudoClass(":show-password", false);
                    passwordTextBox.SetPseudoClass(":hide-password", false);
                }
            }
        );
    }

    private static TextBox? GetParentTextBox(Button button)
    {
        return button.GetVisualAncestors().OfType<TextBox>().FirstOrDefault();
    }

    private static void OnSelectAllClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var textBox = GetParentTextBox(button);

        if (textBox is null)
        {
            return;
        }

        textBox.SelectionStart = 0;
        textBox.SelectionEnd = textBox.Text?.Length ?? 0;
    }

    private static void OnClearAllClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var textBox = GetParentTextBox(button);

        if (textBox is null)
        {
            return;
        }

        textBox.Text = string.Empty;
    }

    private static void OnPasswordClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        var passwordTextBox = GetParentTextBox(button);

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
