using Serilog.Events;
using System;

namespace PngifyMe.ViewModels;

public class LogEntry
{
    public DateTimeOffset Timestamp { get; set; }
    public LogEventLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }

    public string LevelDisplay => Level.ToString();
    
    public string TimestampDisplay => Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
}

