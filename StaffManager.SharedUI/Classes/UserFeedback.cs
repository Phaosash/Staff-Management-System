using StaffManager.Manager.ErrorHandling;
using System.Windows;

namespace StaffManager.SharedUI.Classes;

internal class UserFeedback {
    public static void ErrorFeedback (Exception exception, string message){
        MessageBox.Show(message, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
        LoggingManager.Instance.LogError(exception, message);
    }

    public static void WarningFeedback (string message){
        MessageBox.Show(message, "Application Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        LoggingManager.Instance.LogWarning(message);
    }

    public static void InformationFeedback (string message){
        MessageBox.Show(message, "Application Information", MessageBoxButton.OK, MessageBoxImage.Information);
        LoggingManager.Instance.LogInformation(message);
    }
}