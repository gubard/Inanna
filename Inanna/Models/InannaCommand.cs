using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Inanna.Models;

public sealed partial class InannaCommand : ObservableObject
{
    [ObservableProperty]
    private bool _isEnable;

    public InannaCommand(
        ICommand command,
        object? parameter,
        object content,
        ButtonType type = ButtonType.Normal,
        bool isEnable = true
    )
    {
        Command = command;
        Parameter = parameter;
        Content = content;
        _isEnable = isEnable;
        Type = type;
    }

    public ICommand Command { get; }
    public object? Parameter { get; }
    public object Content { get; }
    public ButtonType Type { get; }
}
