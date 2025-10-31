using Avalonia.Threading;
using PngifyMe.ViewModels;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace PngifyMe.Helpers;

public class LogViewerSink : ILogEventSink
{
    private static readonly object _lockObject = new();
    private const int MAX_LOG_ENTRIES = 10000;

    public static ObservableCollection<LogEntry> LogEntries { get; } = new();

    public void Emit(LogEvent logEvent)
    {
        if (logEvent == null) return;

        Dispatcher.UIThread.Post(() =>
        {
            lock (_lockObject)
            {
                var message = logEvent.MessageTemplate.Render(logEvent.Properties);
                
                if (logEvent.Exception != null)
                {
                    var exceptionBuilder = new StringBuilder();
                    exceptionBuilder.AppendLine(message);
                    exceptionBuilder.AppendLine(logEvent.Exception.ToString());
                    message = exceptionBuilder.ToString();
                }

                var entry = new LogEntry
                {
                    Timestamp = logEvent.Timestamp,
                    Level = logEvent.Level,
                    Message = message,
                    Exception = logEvent.Exception?.ToString()
                };

                LogEntries.Add(entry);

                if (LogEntries.Count > MAX_LOG_ENTRIES)
                {
                    LogEntries.RemoveAt(0);
                }
            }
        });
    }
}

public static class LogViewerForwarder
{
    public static LogViewerSink Sink { get; } = new LogViewerSink();
}

