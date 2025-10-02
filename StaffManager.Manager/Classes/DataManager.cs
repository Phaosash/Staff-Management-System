using StaffManager.Manager.ErrorHandling;

namespace StaffManager.Manager.Classes;

internal class DataManager {
    //  This method attempts to load staff data from a specific CSV file into the provided dictionary,
    //  displaying an error message with exception details if the file cannot be found or loaded.
    public static IDictionary<int, string> InitialiseData (IDictionary<int, string> dictionary){
        try {
            var path = System.IO.Path.Combine(AppContext.BaseDirectory, "Data", "MalinStaffNamesV3.csv");
            var readFile = CsvFileManager.ReadGenericFile(path);
            dictionary = ConvertListToDictionary(readFile, dictionary);
            ProcessDictionaryData(dictionary);
            return dictionary;
        } catch (Exception ex){ 
            LoggingManager.Instance.LogError(ex, "Failed to load the data, the file couldn't be found.");
            return new Dictionary<int, string>();
        }
    }

    //  This method is used to convert the List<string> into an IDictionary<int, string>
    private static IDictionary<int, string> ConvertListToDictionary (List<string> strings, IDictionary<int, string> pairs){
        try {
            foreach (var parts in strings.Select(line => line.Split(',')).Where(parts => parts.Length >= 2)){
                if (int.TryParse(parts[0].Trim(), out int key)){
                    string value = parts[1].Trim();
                    pairs[key] = value;
                }
            }

            return pairs;
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to convert the List to an IDictionary");
            return new Dictionary<int, string>();
        }
    }

    //  This method is used to validate the entires contained in the IDictionary<int, string> to ensure that the data isn't malformed.
    private static IDictionary<int, string> ProcessDictionaryData (IDictionary<int, string> dictionary){
        var validated = new Dictionary<int, string>(); 
        int lineNumber = 1;

        foreach (var kvp in dictionary){
            try {
                double keyStr = kvp.Key;
                string? value = kvp.Value.Trim();

                if (string.IsNullOrWhiteSpace(value)){
                    LoggingManager.Instance.LogWarning($"Line {lineNumber}: One or more fields are empty.");
                    lineNumber++;
                    continue;
                }

                if (double.IsNaN(keyStr)){
                    LoggingManager.Instance.LogWarning($"Line {lineNumber}: Invalid key '{keyStr}' — must be an integer.");
                    lineNumber++;
                    continue;
                }

                if (validated.ContainsKey((int)keyStr)){
                    LoggingManager.Instance.LogWarning($"Line {lineNumber}: Duplicate key '{keyStr}' detected.");
                    lineNumber++;
                    continue;
                }

                validated[(int)keyStr] = value;
            } catch (Exception ex){
                LoggingManager.Instance.LogError(ex, $"Line {lineNumber}: Unexpected error.");
            }

            lineNumber++;
        }

        return validated;
    }

    //  This method attempts to save the staff data dictionary to a specific CSV file, showing an error
    //  message with exception details if the save operation fails.
    public static bool InitialiseDataSave (IDictionary<int, string> dictionary){
        try {
            var path = System.IO.Path.Combine(AppContext.BaseDirectory, "Data", "MalinStaffNamesV3.csv");
            CsvFileManager.SaveIDictionaryToCSV(path, dictionary);
            return true;
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to save the data, the file couldn't be found.");
            return false;
        }
    }

    //  This method generates a new unique staff ID by starting from one above the first key in the dictionary and incrementing until
    //  an unused ID is found, returning -1 if the dictionary is empty or an error occurs.
    private static int CreateNewStaffId (IDictionary<int, string> dict){
        try {
            if (dict == null || dict.Count <= 0){
                LoggingManager.Instance.LogWarning("Unable to create a new staff ID no data was found.");
                return -1;
            }

            int nextID = dict.First().Key + 1;

            while (dict.ContainsKey(nextID)){
                nextID++;
            }

            LoggingManager.Instance.LogInformation($"Successfully created a new ID number: {nextID}");
            return nextID;
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to generate a new unique ID number.");
            return -1;
        }
    }

    //  This method adds a new staff member to the dictionary by generating a unique ID, validating inputs,
    //  and providing user feedback on success or failure during the process.
    public static bool AddStaffMemberToIDictionary (IDictionary<int, string> dict, string name){
        if (dict == null || name == null){
            LoggingManager.Instance.LogWarning("Unable to add new staff member, no data was found");
            return false;
        } 

        try {
            int newId = CreateNewStaffId(dict);

            if (newId == -1){
                LoggingManager.Instance.LogWarning("Failed to generate a new unique ID");
                return false;
            }
        
            dict.Add(newId, name);
            LoggingManager.Instance.LogInformation($"Successfully added a new user to the list with ID: {newId}.");

            return true;
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to add the new staff member to the dictionary.");
            return false;
        }
    }

    //  This method updates the name of a staff member in the dictionary by their ID, validating inputs and providing
    //  feedback on success, missing ID, or any exceptions encountered during the update.
    public static bool UpdateStaffMembersName (IDictionary<int, string> keyValuePairs, int targetId, string updatedName){
        if (keyValuePairs == null || keyValuePairs.Count <= 0 || string.IsNullOrWhiteSpace(updatedName)){
            LoggingManager.Instance.LogWarning("Unable to update the staff members details, no data was found");
            return false;
        }

        try {
            if (keyValuePairs.ContainsKey(targetId)){
                keyValuePairs[targetId] = updatedName;
                LoggingManager.Instance.LogInformation($"Successfully updated the record matching ID: {targetId}");
                return true;
            } else {
                LoggingManager.Instance.LogWarning($"User ID {targetId} was not found in the dictionary.");
                return false;
            }
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Unable to update the target record, encountered an unexpected problem.");
            return false;
        }
    }

    //  This method deletes a staff record from the dictionary by the given ID, providing feedback on successful deletion
    //  or warning if data is missing, and handling exceptions with an error message.
    public static bool DeleteRecord (IDictionary<int, string> keyValuePairs, int targetId){
        if (keyValuePairs == null){
            LoggingManager.Instance.LogWarning("Unable to delete the record no data was found");
            return false;
        }

        try {
            keyValuePairs.Remove(targetId);
            LoggingManager.Instance.LogInformation($"Successfully deleted the user from the dictionary matching ID: {targetId}");
            return true;
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Unable to delete the target record.");
            return false;
        }
    }
}