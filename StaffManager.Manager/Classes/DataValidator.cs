using StaffManager.Manager.ErrorHandling;

namespace StaffManager.Manager.Classes;

internal class DataValidator {
    public static bool DoesApplicationDataExist (IDictionary<int, string> data){
        if (data == null ){
            LoggingManager.Instance.LogWarning("Unable to validate that application data exists, no data was found.");
            return false;
        } 

        return true;
    }

    public static bool MakeSureDeleteIdIsValid (IDictionary<int, string> pairs, int? id){
        if (DoesApplicationDataExist(pairs) && id != null){
            if (DataManager.DeleteRecord(pairs, id.Value)){
                return true;
            } else {
                return false;
            }
        } else {
            LoggingManager.Instance.LogWarning("Unable to delete the record, either the IDictionary or ID was null.");
            return false;
        };
    }

    public static IDictionary<int, string> LoadableDataValidation (IDictionary<int, string> dictionary){
        if (DoesApplicationDataExist(dictionary)){
            return DataManager.InitialiseData(dictionary);
        } else {
            LoggingManager.Instance.LogWarning("Failed to load the data something went wrong.");
            return new Dictionary<int, string>();
        }
    }

    public static bool SaveableDataValidation (IDictionary<int, string> keyValuePairs){
        if (DoesApplicationDataExist(keyValuePairs)){
            return DataManager.InitialiseDataSave(keyValuePairs);
        } else {
            LoggingManager.Instance.LogWarning("Failed to save the changes to the data, no data was found.");
            return false;
        }
    }

    public static bool DataInsertionValidation (IDictionary<int, string> keyValuePairs, string name){
        if (DoesApplicationDataExist(keyValuePairs)){
            return DataManager.AddStaffMemberToIDictionary(keyValuePairs, name);
        } else {
            LoggingManager.Instance.LogWarning("Failed to insert the new team member no data has been loaded.");
            return false;
        }
    }

    public static bool ValidateUpdateData (IDictionary<int, string> keyValuePairs, int id, string name){
        if (DoesApplicationDataExist (keyValuePairs)){
            return DataManager.UpdateStaffMembersName(keyValuePairs, id, name);
        } else {
            LoggingManager.Instance.LogWarning("Unable to validate the data for updating the employees details.");
            return false;
        }
    }
}