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

    public static void MakeSureDeleteIdIsValid (IDictionary<int, string> pairs, int? id){
        if (DoesApplicationDataExist(pairs)){
            //  TODO: Validate and call delete method
        }
    }

    public static IDictionary<int, string> LoadableDataValidation (){
        
        //  Return a new standard dicationary if something goes wrong
        return new Dictionary<int, string> ();
    }

    public static bool SavableDataValidation (){
        //  TODO: Flesh this out when data maanger fully implemented
        return true;
    }
}