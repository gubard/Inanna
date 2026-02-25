using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Inanna.Models;

public sealed class LogNotify : ObservableObject
{
    public LogNotify(
        int eventId,
        DateTimeOffset dateTime,
        LogLevel level,
        string text,
        object? state,
        Exception? exception
    )
    {
        EventId = eventId;
        DateTime = dateTime;
        Level = level;
        Text = text;
        State = state;
        Exception = exception;
    }

    public int EventId { get; }
    public DateTimeOffset DateTime { get; }
    public LogLevel Level { get; }
    public string Text { get; }
    public object? State { get; }
    public Exception? Exception { get; }
}
