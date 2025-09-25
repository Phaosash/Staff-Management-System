using ErrorLogging;
using System.Windows;

namespace StaffManager.Classes;

internal class UserFeedback {
    //  This method displays an error message to the user in a message box with a specified error type
    //  and logs the same message as a warning using the LoggingManager.
    public static void DisplayErrorMessage (string message, string errorType){
        MessageBox.Show(message, errorType, MessageBoxButton.OK, MessageBoxImage.Error);
        LoggingManager.Instance.LogWarning(message);
    }

    //  This method displays an error message to the user in a message box with a specified error type
    //  and then logs the same message with the exception using the LoggingManager.
    public static void DisplayErrorMessageWithException (string message, string errorType, Exception exception){
        MessageBox.Show(message, errorType, MessageBoxButton.OK, MessageBoxImage.Error);
        LoggingManager.Instance.LogError(exception, message);
    }

    //  This method displays a warning to the user in a message box with a specified warning type
    //  and then logs the warning using the same message using the logging manager.
    public static void DisplayWarning(string message, string warningType){
        MessageBox.Show(message, warningType, MessageBoxButton.OK, MessageBoxImage.Warning);
        LoggingManager.Instance.LogWarning(message);
    }

    //  This method displays general information messages to the use like successful completion of operations
    //  these messages are also logged using the logging manager.
    public static void DisplayInformation (string message, string type){
        MessageBox.Show(message, type, MessageBoxButton.OK, MessageBoxImage.Information);
        LoggingManager.Instance.LogInformation(message);
    }

    //  This method is used to log information about the application without providing direct feedback
    //  to the user.
    public static void LogApplicationInformation (string details){
        LoggingManager.Instance.LogInformation(details);
    }

    //  This method is used to silently log errors through the Logging Manager.
    public static void LogError (string message, Exception exception){
        LoggingManager.Instance.LogError(exception, message);
    }
}