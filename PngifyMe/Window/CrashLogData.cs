using System;

namespace PngifyMe;

public class CrashLogData
{
    public string? ExceptionType { get; set; }
    public string? Message { get; set; }
    public string? StackTrace { get; set; }
    public string? Source { get; set; }
    public CrashLogData? InnerException { get; set; }
    public DateTime Timestamp { get; set; }

    public static CrashLogData FromException(Exception? ex)
    {
        if (ex == null)
        {
            return new CrashLogData
            {
                ExceptionType = "Unknown",
                Message = "An unknown error occurred.",
                Timestamp = DateTime.UtcNow
            };
        }

        return new CrashLogData
        {
            ExceptionType = ex.GetType().FullName,
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            Source = ex.Source,
            InnerException = ex.InnerException != null ? FromException(ex.InnerException) : null,
            Timestamp = DateTime.UtcNow
        };
    }

    public Exception ToException()
    {
        var exception = new Exception(Message ?? "Unknown error");
        
        // Set stack trace via reflection
        var stackTraceField = typeof(Exception).GetField("_stackTraceString",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        stackTraceField?.SetValue(exception, StackTrace);

        // Set inner exception if present
        if (InnerException != null)
        {
            var innerEx = InnerException.ToException();
            var innerExceptionField = typeof(Exception).GetField("_innerException",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            innerExceptionField?.SetValue(exception, innerEx);
        }

        return exception;
    }
}

