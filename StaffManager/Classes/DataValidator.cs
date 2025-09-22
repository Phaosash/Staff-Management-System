namespace StaffManager.Classes;

internal class DataValidator {
    public static void ValidateNewUserData (IDictionary<int, string> pairs, string name){
        if (string.IsNullOrWhiteSpace(name) || pairs == null){
            UserFeedback.DisplayErrorMessage("Unable to add new staff member, a Name is required", "No Data Value");
            return;
        }

        DataManager.AddStaffMemberToIDictionary(pairs, name);
    }

    public static void ValidateUpdateData (IDictionary<int, string> pairs, string name, int? id){
        if (pairs == null || string.IsNullOrWhiteSpace(name) || !id.HasValue){
            UserFeedback.DisplayErrorMessage("Unable to Update the employee record, either the name or ID couldn't be validated.", "No Data Value");
            return;
        }
        
        DataManager.UpdateStaffMembersName(pairs, id.Value, name);
    }

    public static void ValidateDeleteData (IDictionary<int, string> pairs, int? id){
        if (pairs == null || !id.HasValue){
            UserFeedback.DisplayErrorMessage("Unable to Delete the employee record, the ID couldn't be validated.", "No Data Value");
            return;
        }

        DataManager.DeleteRecord(pairs, id.Value);
    }

    public static void ValidateLoadableData (IDictionary<int, string> pairs){
        if (pairs != null){
            DataManager.InitialiseData(pairs);
        } else {
            UserFeedback.DisplayErrorMessage("Failed to initialise the data", "Data Initialisation Error");
        }
    }
}