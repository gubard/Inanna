using Avalonia.Collections;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed class AdaptiveButtonsViewModel : ViewModelBase
{
    public AdaptiveButtonsViewModel(
        IAvaloniaReadOnlyList<InannaCommand> commands,
        ISafeExecuteWrapper safeExecuteWrapper
    )
        : base(safeExecuteWrapper)
    {
        Commands = commands;
    }

    public IAvaloniaReadOnlyList<InannaCommand> Commands { get; }
}
