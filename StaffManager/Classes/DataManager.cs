using StaffManager.DataModels;
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
        } else {
            int newId = CreateNewStaffId(dict);

            if (newId == -1){
                UserFeedback.DisplayErrorMessage("Failed to generate a new unique ID", "Invalid Id Value");
            } else {
                dict.Add(newId, name);
                UserFeedback.DisplayInformation("Successfully added a new user to the list." , "Successful Completion");
            }
        }
    }
}