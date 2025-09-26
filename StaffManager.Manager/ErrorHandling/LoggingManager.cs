using Microsoft.Extensions.Logging;

namespace StaffManager.Manager.ErrorHandling;

public sealed class LoggingManager {
    private const int DefaultRetentionPeriod = 7;
    private static readonly Lazy<LoggingManager> _instance = new(() => new LoggingManager(DefaultRetentionPeriod));
    private readonly FileManager _fileManager;
    private readonly int _fileRetentionPeriod;

    private LoggingManager (int fileRetentionPeriod){
        _fileRetentionPeriod = fileRetentionPeriod < 0 ? DefaultRetentionPeriod : fileRetentionPeriod;
        _fileManager = new FileManager(_fileRetentionPeriod);
    }

    public static LoggingManager Instance => _instance.Value;

    public void LogInformation (string message){
        _fileManager.Log(LogLevel.Information, message, null, (state, exception) => $"Message: {state}");
    }

    public void LogError (Exception ex, string message){
        _fileManager.Log(LogLevel.Error, message, ex, (state, exception) => $"{state}: {exception?.Message}\nStackTrace: {exception?.StackTrace}");
    }

    public void LogWarning (string message){
        _fileManager.Log(LogLevel.Warning, message, null, (state, exception) => $"Warning: {state}");
    }
}