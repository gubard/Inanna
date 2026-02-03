using Avalonia.Controls;

namespace Inanna.Helpers;

public static class TextBoxExtension
{
    public static void FocusCaretIndex(this TextBox textBox)
    {
        textBox.Focus();
        textBox.CaretIndex = textBox.Text?.Length ?? 0;
    }
}
