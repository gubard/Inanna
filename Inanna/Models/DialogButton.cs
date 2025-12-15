using System.Windows.Input;

namespace Inanna.Models;

public class DialogButton
{
    public DialogButton(
        object content,
        ICommand command,
        object? commandParameter,
        DialogButtonType type
    )
    {
        Content = content;
        Command = command;
        Type = type;
        CommandParameter = commandParameter;
    }

    public object Content { get; }
    public ICommand Command { get; }
    public object? CommandParameter { get; }
    public DialogButtonType Type { get; }
}
