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
        object icon,
        ButtonType type = ButtonType.Normal,
        bool isEnable = true
    )
    {
        Command = command;
        Parameter = parameter;
        Content = content;
        Icon = icon;
        _isEnable = isEnable;
        Type = type;
    }

    public ICommand Command { get; }
    public object? Parameter { get; }
    public object Content { get; }
    public object Icon { get; }
    public ButtonType Type { get; }
}
