namespace StaffManager.Classes;

internal class DataManager {
    //  This method attempts to load staff data from a specific CSV file into the provided dictionary,
    //  displaying an error message with exception details if the file cannot be found or loaded.
    public static IDictionary<int, string> InitialiseData (IDictionary<int, string> dictionary){
        try {
            var path = System.IO.Path.Combine(AppContext.BaseDirectory, "Data", "MalinStaffNamesV3.csv");
            var readFile = FileManager.ReadGenericFile(path);
            dictionary = ConvertListToDictionary(readFile, dictionary);
            ProcessDictionaryData(dictionary);
            return dictionary;
        } catch (Exception ex){ 
            UserFeedback.DisplayErrorMessageWithException("Failed to load the data, the file couldn't be found.", "Data Initialisation Error", ex);
            return new Dictionary<int, string>();
        }
    }

    //  This method is used to convert the List<string> into an IDictionary<int, string>
    private static IDictionary<int, string> ConvertListToDictionary (List<string> strings, IDictionary<int, string> pairs){
        foreach (var parts in strings.Select(line => line.Split(',')).Where(parts => parts.Length >= 2)){
            if (int.TryParse(parts[0].Trim(), out int key)){
                string value = parts[1].Trim();
                pairs[key] = value;
            }
        }

        return pairs;
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
                    UserFeedback.DisplayErrorMessage($"Line {lineNumber}: One or more fields are empty.", "Validation Error");
                    lineNumber++;
                    continue;
                }

                if (double.IsNaN(keyStr)){
                    UserFeedback.DisplayErrorMessage($"Line {lineNumber}: Invalid key '{keyStr}' — must be an integer.", "Validation Error");
                    lineNumber++;
                    continue;
                }

                if (validated.ContainsKey((int)keyStr)){
                    UserFeedback.DisplayErrorMessage($"Line {lineNumber}: Duplicate key '{keyStr}' detected.", "Validation Error");
                    lineNumber++;
                    continue;
                }

                validated[(int)keyStr] = value;
            } catch (Exception ex){
                UserFeedback.DisplayErrorMessageWithException($"Line {lineNumber}: Unexpected error.", "Processing Error", ex);
            }

            lineNumber++;
        }

        return validated;
    }

    //  This method attempts to save the staff data dictionary to a specific CSV file, showing an error
    //  message with exception details if the save operation fails.
    public static void InitialiseDataSave (IDictionary<int, string> dictionary){
        try {
            var path = System.IO.Path.Combine(AppContext.BaseDirectory, "Data", "MalinStaffNamesV3.csv");
            FileManager.SaveIDictionaryToCSV(path, dictionary);
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Failed to save the data, the file couldn't be found.", "Data Save Error", ex);
        }
    }

    //  This method generates a new unique staff ID by starting from one above the first key in the dictionary and incrementing until
    //  an unused ID is found, returning -1 if the dictionary is empty or an error occurs.
    private static int CreateNewStaffId (IDictionary<int, string> dict){
        try {
            if (dict == null || dict.Count <= 0){
                return -1;
            }

            int nextID = dict.First().Key + 1;

            while (dict.ContainsKey(nextID)){
                nextID++;
            }

            return nextID;
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Failed to generate a new unique ID number.", "ID Generation Error", ex);
            return -1;
        }
    }

    //  This method adds a new staff member to the dictionary by generating a unique ID, validating inputs,
    //  and providing user feedback on success or failure during the process.
    public static void AddStaffMemberToIDictionary (IDictionary<int, string> dict, string name){
        if (dict == null || name == null){
            UserFeedback.DisplayErrorMessage("Unable to add new staff member, no data was found", "No Data Error");
            return;
        } 

        try {
            int newId = CreateNewStaffId(dict);

            if (newId == -1){
                UserFeedback.DisplayErrorMessage("Failed to generate a new unique ID", "Invalid Id Value");
                return;
            }
        
            dict.Add(newId, name);
            UserFeedback.DisplayInformation($"Successfully added a new user to the list with ID: {newId}." , "Successful Completion");
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Failed to add the new staff member to the dictionary.", "Data Processing Error", ex);
        }
    }

    //  This method updates the name of a staff member in the dictionary by their ID, validating inputs and providing
    //  feedback on success, missing ID, or any exceptions encountered during the update.
    public static void UpdateStaffMembersName (IDictionary<int, string> keyValuePairs, int targetId, string updatedName){
        if (keyValuePairs == null || keyValuePairs.Count <= 0 || string.IsNullOrWhiteSpace(updatedName)){
            UserFeedback.DisplayWarning("Unable to update the staff members details, no data was found", "Missing Details");
            return;
        }

        try {
            if (keyValuePairs.ContainsKey(targetId)){
                keyValuePairs[targetId] = updatedName;
                UserFeedback.DisplayInformation($"Successfully updated the record matching ID: {targetId}", "Success");
            } else {
                UserFeedback.DisplayWarning($"User ID {targetId} was not found in the dictionary.", "Missing Details");
            }
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Unable to update the target record, encountered an unexpected problem.", "Missing Details", ex);
        }
    }

    //  This method deletes a staff record from the dictionary by the given ID, providing feedback on successful deletion
    //  or warning if data is missing, and handling exceptions with an error message.
    public static void DeleteRecord (IDictionary<int, string> keyValuePairs, int targetId){
        if (keyValuePairs == null){
            UserFeedback.DisplayWarning("Unable to delete the record no data was found", "Missing Details");
            return;
        }

        try {
            keyValuePairs.Remove(targetId);
            UserFeedback.DisplayInformation("Successfully deleted the user from the dictionary", "Success");
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Unable to delete the target record.", "Missing Details", ex);
        }
    }
}