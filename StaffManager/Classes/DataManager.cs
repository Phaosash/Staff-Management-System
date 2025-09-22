using System.IO;

namespace StaffManager.Classes;

internal class DataManager {
    public static void InitialiseData (IDictionary<int, string> dictionary){
        try {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "MalinStaffNamesV3.csv");
            FileManager.LoadFromCsv(path, dictionary);
        } catch (Exception ex){ 
            UserFeedback.DisplayErrorMessageWithException("Failed to load the data, the file couldn't be found.", "Data Initialisation Error", ex);
        }
    }

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

    public static void AddStaffMemberToIDictionary (IDictionary<int, string> dict, string name){
        if (dict == null || name == null){
            UserFeedback.DisplayErrorMessage("Unable to add new staff member, no data was found", "No Data Error");
            return;
        } 

        int newId = CreateNewStaffId(dict);

        if (newId == -1){
            UserFeedback.DisplayErrorMessage("Failed to generate a new unique ID", "Invalid Id Value");
            return;
        }
        
        dict.Add(newId, name);
        UserFeedback.DisplayInformation($"Successfully added a new user to the list with ID: {newId}." , "Successful Completion");
    }

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