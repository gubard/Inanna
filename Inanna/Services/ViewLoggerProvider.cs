using Avalonia.Threading;
using Inanna.Ui;
using Microsoft.Extensions.Logging;
using IServiceProvider = Gaia.Services.IServiceProvider;

namespace Inanna.Services;

public sealed class ViewLoggerProvider : ILoggerProvider
{
    public ViewLoggerProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new ViewLogger(_serviceProvider.GetService<LogsViewModel>());
    }

    public void Dispose() { }

    private readonly IServiceProvider _serviceProvider;

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
