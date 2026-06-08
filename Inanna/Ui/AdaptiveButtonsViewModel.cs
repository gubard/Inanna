using Avalonia.Collections;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed class AdaptiveButtonsViewModel : ViewModelBase
{
    public AdaptiveButtonsViewModel(
        IAvaloniaReadOnlyList<InannaCommand> commands,
        ViewModelServices services
    )
        : base(services)
    {
        Commands = commands;
    }

    public IAvaloniaReadOnlyList<InannaCommand> Commands { get; }
}
