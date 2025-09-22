using ErrorLogging;
using System.Windows;

namespace StaffManager.Classes;

internal class UserFeedback {
    public static void DisplayErrorMessage (string message, string errorType){
        MessageBox.Show(message, errorType, MessageBoxButton.OK, MessageBoxImage.Error);
        LoggingManager.Instance.LogWarning(message);
    }

    public static void DisplayErrorMessageWithException (string message, string errorType, Exception exception){
        MessageBox.Show(message, errorType, MessageBoxButton.OK, MessageBoxImage.Error);
        LoggingManager.Instance.LogError(exception, message);
    }

    public static void DisplayWarning(string message, string warningType){
        MessageBox.Show(message, warningType, MessageBoxButton.OK, MessageBoxImage.Warning);
        LoggingManager.Instance.LogWarning(message);
    }

    public static void DisplayInformation (string message, string type){
        MessageBox.Show(message, type, MessageBoxButton.OK, MessageBoxImage.Information);
        LoggingManager.Instance.LogInformation(message);
    }
}