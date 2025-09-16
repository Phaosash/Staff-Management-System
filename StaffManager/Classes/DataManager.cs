using ErrorLogging;
using StaffManager.DataModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace StaffManager.Classes;

internal class DataManager {
    public static void InitialiseData (IDictionary<int, string> dictionary){
        try {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "MalinStaffNamesV3.csv");
            FileManager.LoadFromCsv(path, dictionary);
        } catch (Exception ex){ 
            LoggingManager.Instance.LogError(ex, "Failed to load the data, the file couldn't be found.");
        }
    }

    public static void SaveDataToCsv (IDictionary<int, string> keyValuePairs){
        try {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "MalinStaffNamesV3.csv");
            FileManager.SaveToCSV(path, keyValuePairs);
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, $"Failed to load the data, the file couldn't be found. {ex}");
        }
    }

    public static void DeleteEntryViaKey (IDictionary<int, string> keyValuePairs, string deletionKey){
        try {
            if (int.TryParse(deletionKey, out var deletion)){
                keyValuePairs.Remove(deletion);
                LoggingManager.Instance.LogInformation($"Successfully deleted the staff member record for ID: {deletionKey}");
                MessageBox.Show($"Successfully deleted the record for ID: {deletionKey}.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            } else {
                LoggingManager.Instance.LogWarning($"Failed to delete the record for ID: {deletionKey}.");
                MessageBox.Show($"Failed to delete the record for ID: {deletionKey}.", "Failure", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to delete the staff member.");
            MessageBox.Show("Encountered an unexpected problem trying to delete the record, please try again.", "Data Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public static ObservableCollection<StaffMember> FindStaffMember (IDictionary<int, string> dictionary, string searchQuery){
        var matchingStaffMembers = new ObservableCollection<StaffMember>();

        try {
            if (int.TryParse(searchQuery, out int key)){
                if (dictionary.TryGetValue(key, out string? value)){
                    matchingStaffMembers.Add(new StaffMember { Id = key, Name = value });
                    LoggingManager.Instance.LogInformation($"A matching staff member was found with ID: {key}\n searchQueary = {searchQuery}");
                } else {
                    LoggingManager.Instance.LogInformation($"No staff member found with ID: {key}\n searchQuery = {searchQuery}");
                }
            } else {
                LoggingManager.Instance.LogInformation($"Search is being conducted with searchQueary = {searchQuery}");

                foreach (var entry in dictionary){
                    if (entry.Value.Trim().Contains(searchQuery.Trim(), StringComparison.OrdinalIgnoreCase)){
                        matchingStaffMembers.Add(new StaffMember { Id = entry.Key, Name = entry.Value });
                    }
                }

                LoggingManager.Instance.LogInformation($"Returned an ObservableCollection<StaffMember> that contained: {matchingStaffMembers.Count} entries");
            }

            return matchingStaffMembers;
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to find a valid staff member in the dictionary.");
            return [];
        }
    }

    private static bool CheckStartingNumbers (int number){
        return number.ToString().StartsWith("77");
    }

    public static bool ValidateStaffCreationFields (int id, string name){
        if (CheckStartingNumbers(id)){
            LoggingManager.Instance.LogWarning("An invalid ID number was eneted for the new staff memeber.");
            return false;
        }

        if (string.IsNullOrEmpty(name)){
            LoggingManager.Instance.LogWarning("An invalid string Name was eneted for the new staff memeber.");
            return false;
        }

        return true;
    }

    public static bool CheckDataExists (IDictionary<int, string> dictionary, string searchTarget){
        if (dictionary == null){
            LoggingManager.Instance.LogWarning("MasterFile is null.");
            MessageBox.Show("Unable to search for the staff memeber the data is missing.", "No Data Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrEmpty(searchTarget)){
            LoggingManager.Instance.LogWarning("SearchTerm is null.");
            MessageBox.Show("Unable to search for the staff member, invalid search term.", "Invalid Search Term", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }
}