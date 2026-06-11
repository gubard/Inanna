using Avalonia.Collections;
using Inanna.Models;

namespace Inanna.Ui;

public sealed class LogsViewModel : ViewModelBase
{
    public LogsViewModel(ViewModelServices services)
        : base(services) { }

    public AvaloniaList<LogNotify> Logs { get; } = new();
}
