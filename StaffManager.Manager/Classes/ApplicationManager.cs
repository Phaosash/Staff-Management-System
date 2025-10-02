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
        return DataValidator.SaveableDataValidation(_masterFile);
    }

    public bool InitialiseDataInsertion (string nameToInsert){
        return DataValidator.DataInsertionValidation(_masterFile, nameToInsert);
    }

    public bool InitialiseDataDeletion (int id){
        return DataValidator.MakeSureDeleteIdIsValid(_masterFile, id);
    }

    public bool InitialiseDataUpdate (int id, string name){
        return DataValidator.ValidateUpdateData(_masterFile, id, name);
    }

    private bool InitialiseDataLoad (){
        _masterFile = DataValidator.LoadableDataValidation(_masterFile);

        if (_masterFile.Count > 0){
            return true;
        } else {
            return false;
        }
    }
}