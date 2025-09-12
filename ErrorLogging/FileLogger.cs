using Microsoft.Extensions.Logging;

namespace ErrorLogging;

internal class FileLogger {
    private readonly Lock _fileLock = new();

    //  This method logs a message with a specified log level, state, and optional exception by formatting the log entry with a timestamp,
    //  then asynchronously writing it to a file in a thread-safe manner using a lock.
    public void Log<TState> (LogLevel logLevel, TState state, Exception? exception, Func<TState, Exception?, string> formatter){
        ArgumentNullException.ThrowIfNull(formatter);

        var logMessage = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss} [{logLevel}] {formatter(state, exception)}";

        Task.Run(() => {
            lock (_fileLock)
            {
                LogToFile(logMessage);
            }
        });
    }

    //  This method writes a log message to a "logfile.txt" inside a logs directory in the application's base path,
    //  creating the directory if it doesn't exist, and catches any exceptions during the write operation to output
    //  an error to the console.
    private static void LogToFile (string logMessage){
        try {
            string logsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

            if (!Directory.Exists(logsDirectory)){
                Directory.CreateDirectory(logsDirectory);
            }

            string filePath = Path.Combine(logsDirectory, "logfile.txt");
            File.AppendAllText(filePath, logMessage + Environment.NewLine);
        } catch (Exception ex){
            Console.WriteLine($"Error writing to log file: {ex.Message}");
        }
    }
}