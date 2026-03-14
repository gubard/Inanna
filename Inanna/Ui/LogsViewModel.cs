using Avalonia.Collections;
using Inanna.Models;
using Inanna.Services;

namespace Inanna.Ui;

public sealed class LogsViewModel : ViewModelBase
{
    public LogsViewModel(ISafeExecuteWrapper safeExecuteWrapper)
        : base(safeExecuteWrapper) { }

    public AvaloniaList<LogNotify> Logs { get; } = new();
}
