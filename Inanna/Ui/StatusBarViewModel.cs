using Avalonia.Collections;
using Inanna.Models;

namespace Inanna.Ui;

public sealed class StatusBarViewModel : ViewModelBase
{
    public StatusBarViewModel()
    {
        Statuses = new();
    }

    public AvaloniaList<object> Statuses { get; }
}
