using System.Windows.Input;

namespace Inanna.Models;

public sealed class SpravaCommand
{
    public SpravaCommand(ICommand command, object? parameter, object content)
    {
        Command = command;
        Parameter = parameter;
        Content = content;
    }

    public ICommand Command { get; }
    public object? Parameter { get; }
    public object Content { get; }
   
}