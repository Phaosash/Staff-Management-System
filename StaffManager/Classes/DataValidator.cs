namespace StaffManager.Classes;

internal class DataValidator {
    //  This method checks if a valid name and dictionary are provided before adding a new staff member to the dictionary,
    //  showing an error message if the name is missing or the dictionary is null.
    public static void ValidateNewUserData (IDictionary<int, string> pairs, string name){
        if (string.IsNullOrWhiteSpace(name) || pairs == null){
            UserFeedback.DisplayErrorMessage("Unable to add new staff member, a Name is required", "No Data Value");
            return;
        }

        DataManager.AddStaffMemberToIDictionary(pairs, name);
    }

    //  This method validates that the dictionary, name, and staff ID are all provided before updating a staff member's name,
    //  displaying an error if any required data is missing or invalid.
    public static void ValidateUpdateData (IDictionary<int, string> pairs, string name, int? id){
        if (pairs == null || string.IsNullOrWhiteSpace(name) || !id.HasValue){
            UserFeedback.DisplayErrorMessage("Unable to Update the employee record, either the name or ID couldn't be validated.", "No Data Value");
            return;
        }
            
        DataManager.UpdateStaffMembersName(pairs, id.Value, name);
    }

    //  This method ensures the dictionary and staff ID are valid before deleting a staff record, showing an error message
    //  if the ID is missing or the dictionary is null.
    public static void ValidateDeleteData (IDictionary<int, string> pairs, int? id){
        if (pairs == null || !id.HasValue){
            UserFeedback.DisplayErrorMessage("Unable to Delete the employee record, the ID couldn't be validated.", "No Data Value");
            return;
        }

        DataManager.DeleteRecord(pairs, id.Value);
    }

    //  This method attempts to initialize data using the provided dictionary if it’s not null, otherwise it displays an error
    //  indicating data initialization failure.
    public static IDictionary<int, string> ValidateLoadableData (IDictionary<int, string> pairs){
        if (pairs == null){
            UserFeedback.DisplayErrorMessage("The selected dictionary is null.", "Data Validation Error");
            return new Dictionary<int, string>();
        }

        return DataManager.InitialiseData(pairs);
    }

    //  This method checks if the data dictionary is valid before proceeding to initialize the save process,
    //  warning the user if the data cannot be validated.
    public static void ValidateDataForSave (IDictionary<int, string> pairs){
        if (pairs == null){
            UserFeedback.DisplayWarning("Unable to save the changes, the data couldn't be validated.", "Data Validation Error");
            return;
        }

        DataManager.InitialiseDataSave(pairs);
    }
}