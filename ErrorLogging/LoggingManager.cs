using Microsoft.Extensions.Logging;

namespace ErrorLogging;

public sealed class LoggingManager {
    private static readonly Lazy<LoggingManager> _instance = new(() => new LoggingManager(new FileLogger()));
    private readonly FileLogger _fileLogger;

    private LoggingManager (FileLogger fileLogger){
        _fileLogger = fileLogger ?? throw new ArgumentNullException(nameof(fileLogger));
    }

    public static LoggingManager Instance => _instance.Value;

    //  This method logs an informational message using an internal file logger, formatting the log entry to include the provided message.
    public void LogInformation (string message){
        _fileLogger.Log(LogLevel.Information, message, null, (state, exception) => $"Message: {state}");
    }

    //  This method logs an error message along with exception details using an internal file logger, including the exception message and stack trace for better diagnostics.
    public void LogError (Exception ex, string message){
        _fileLogger.Log(LogLevel.Error, message, ex, (state, exception) => $"{state}: {exception?.Message}\nStackTrace: {exception?.StackTrace}");
    }

    //  This method logs a warning message using an internal file logger, prefixing the log entry with "Warning:" to highlight its severity.
    public void LogWarning (string message){
        _fileLogger.Log(LogLevel.Warning, message, null, (state, exception) => $"Warning: {state}");
    }

    //  This method logs a warning message along with exception details using an internal file logger, including the exception message to provide additional context.
    public void LogWarningWithException (Exception ex, string message){
        _fileLogger.Log(LogLevel.Warning, message, ex, (state, exception) => $"Warning: {state}. Exception: {exception?.Message}");
    }
}