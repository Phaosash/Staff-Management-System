using StaffManager.Manager.ErrorHandling;

namespace StaffManager.Manager.Classes;

public class ApplicationManager {
    private IDictionary<int, string> _masterFile;

    public ApplicationManager (IDictionary<int, string> theDictionary){
        _masterFile = theDictionary;
        InitialiseDataLoad();
    }

    public bool CheckDataExists (){
        return DataValidator.DoesApplicationDataExist(_masterFile);
    }

    public IDictionary<int, string> GetMasterFile (){
        if (!DataValidator.DoesApplicationDataExist(_masterFile)){
            return new Dictionary<int, string>();
        } else {
            return _masterFile;
        }
    }

    public bool InitialiseDataSave (){
        //  TODO: Save logic
        return true;
    }

    public bool InitialiseDataInsertion (string nameToInsert){
        //  TODO: Adding new user
        return true;
    }

    public bool InitialiseDataDeletion (int id){
        DataValidator.MakeSureDeleteIdIsValid(_masterFile, id);
        return true;
    }

    public bool InitialiseDataUpdate (int? id, string name){
        //  TODO: Data Update
        return true;
    }

    private void InitialiseDataLoad (){
        if (DataValidator.DoesApplicationDataExist(_masterFile)){
            _masterFile = DataValidator.LoadableDataValidation();
        } else {
            _masterFile = new Dictionary<int, string>();
        }
    }
}