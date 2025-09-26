using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace StaffManager.Manager.ErrorHandling;

internal class FileManager {
    private readonly Lock _fileLock = new();
    private readonly int _retentionPeriod;
    private readonly string _logsDirectory;

    public FileManager (int logRetentionPeriod){
        _retentionPeriod = logRetentionPeriod;

        _logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
        CleanUpOldLogs(_retentionPeriod);
    }

    public void Log<TState> (LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter,
                            [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0){
        ArgumentNullException.ThrowIfNull(formatter);

        var logMessage = BuildLogMessage(logLevel, state, exception, formatter, memberName, filePath, lineNumber);

        WriteLogToFileAsync(logMessage);
    }

    private static string BuildLogMessage<TState>(LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter, string memberName, string filePath, int lineNumber){
        return $"{DateTime.Now:dd-MM-yyyy HH:mm:ss} [{logLevel}] {formatter(state, exception)}" + $"\nSource: {Path.GetFileName(filePath)}.{memberName} Line: {lineNumber}";
    }

    private async void WriteLogToFileAsync (string logMessage){
        try {
            await Task.Yield();

            lock (_fileLock){
                LogToFile(logMessage);
            }
        } catch (Exception ex){
            Debug.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }

    private void LogToFile (string logMessage){
        try {
            if (!Directory.Exists(_logsDirectory)){
                Directory.CreateDirectory(_logsDirectory);
            }

            string filePath = Path.Combine(_logsDirectory, $"ApplicationLogs_{DateTime.Now:dd-MM-yyyy}.txt");
            File.AppendAllText(filePath, logMessage + Environment.NewLine);
        } catch (Exception ex){
            Debug.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }

    private void CleanUpOldLogs (int retentionDays){
        try {
            if (Directory.Exists(_logsDirectory)){
                var logFiles = Directory.GetFiles(_logsDirectory, "ApplicationLogs_*.txt");

                foreach (var logFile in logFiles){
                    DateTime creationDate = File.GetCreationTime(logFile);

                    if (creationDate < DateTime.Now.AddDays(-retentionDays)){
                        File.Delete(logFile);
                    }
                }
            }
        } catch (Exception ex){
            Debug.WriteLine($"Error cleaning up old log files: {ex.Message}");
        }
    }
}