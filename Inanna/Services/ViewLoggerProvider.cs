using Avalonia.Threading;
using Inanna.Ui;
using Microsoft.Extensions.Logging;

namespace Inanna.Services;

public sealed class ViewLoggerProvider : ILoggerProvider
{
    public ViewLoggerProvider(LogsViewModel logsViewModel)
    {
        _logsViewModel = logsViewModel;
    }

    public ILogger CreateLogger(string categoryName) => new ViewLogger(_logsViewModel);

    public void Dispose() { }

    private readonly LogsViewModel _logsViewModel;

    private sealed class ViewLogger : ILogger
    {
        private readonly LogsViewModel _logsViewModel;

        public ViewLogger(LogsViewModel logsViewModel)
        {
            _logsViewModel = logsViewModel;
        }

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter
        )
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            Dispatcher.UIThread.Post(() =>
                _logsViewModel.Logs.Add(
                    new(
                        eventId.Id,
                        DateTimeOffset.Now,
                        logLevel,
                        formatter(state, exception),
                        state,
                        exception
                    )
                )
            );
        }
    }
}
