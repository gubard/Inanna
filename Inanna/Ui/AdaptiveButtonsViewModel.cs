using Avalonia.Collections;
using Inanna.Models;

namespace Inanna.Ui;

public sealed class AdaptiveButtonsViewModel : ViewModelBase
{
    public AdaptiveButtonsViewModel(IAvaloniaReadOnlyList<InannaCommand> commands)
    {
        Commands = commands;
    }

    public IAvaloniaReadOnlyList<InannaCommand> Commands { get; }
}
