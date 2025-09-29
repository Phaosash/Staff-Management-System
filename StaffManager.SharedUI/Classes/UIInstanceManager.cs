using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StaffManager.Manager.Classes;
using StaffManager.SharedUI.DataModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StaffManager.SharedUI.Classes;

public partial class UIInstanceManager: ObservableObject {
    public BindingObject DataBindings { get; } = new BindingObject();
    public event Action? RequestNewWindow;

    //  This needs to be seperate from the Application close to avoid it acceddently
    //  closing the application at the same time as the window.
    public event Action? RequestWindowClose;
    public event Action? RequestApplicationClose;

    //private enum ObjectFieldToClear { IdField, NameField }
    private readonly ApplicationManager _manager;
    private readonly DataBingingsHelpers _helpers;

    //  Suspect I need to pass the IDicitionary to the backend through the injection
    public UIInstanceManager (IDictionary<int, string> keyValuePairs, ApplicationManager applicationManager){
        _manager = applicationManager;
        _helpers = new DataBingingsHelpers(DataBindings);

        DataBindings.ReadOnlyObject = _manager.GetMasterFile();

        DataBindings.PropertyChanged += DataBindings_PropertyChanged;
    }

    private void DataBindings_PropertyChanged (object? sender, PropertyChangedEventArgs e){
        try {
            switch (e.PropertyName){
                case nameof(DataBindings.SearchTerm):
                    FilterLoadedData(DataBindings.SearchTerm);
                    break;
                case nameof(DataBindings.SelectedObject):
                    _helpers.OnSelectedObjectChanged(DataBindings.SelectedObject);
                    break;
            }
        } catch (Exception ex){
            _helpers.ProvideGeneralErrorFeedback(ex, "Encnountered an unexpected property change request.");
        }
    }

    private void FilterLoadedData (string filter){
        try {
            if (!_manager.CheckDataExists()){
                _helpers.ProvideGeneralWarningFeedback("Failed to filter the data no data was found");
            } else {
                filter = filter.Trim() ?? string.Empty;

                IEnumerable<SelectableObjects> filteredObjects;

                if (string.IsNullOrWhiteSpace(filter)){
                    filteredObjects = [];
                } else if (int.TryParse(filter, out _)){
                     filteredObjects = _manager.GetMasterFile().Where(kvp => kvp.Key.ToString().StartsWith(filter)).Select(kvp => new SelectableObjects { Id = kvp.Key, Name = kvp.Value });
                } else {
                    filteredObjects = _manager.GetMasterFile().Where(kvp => kvp.Value.Contains(filter, StringComparison.OrdinalIgnoreCase)).Select(kvp => new SelectableObjects { Id = kvp.Key, Name = kvp.Value });
                }

                DataBindings.SelectableObjects = new ObservableCollection<SelectableObjects>(filteredObjects);
            }
        } catch (Exception ex){
            _helpers.ProvideGeneralErrorFeedback(ex, "Encountered an unexpected problem while filtering the data.");
        }
    }

    //private void OnSelectedObjectChanged (SelectableObjects value){
    //    try {
    //        if (value != null){
    //            DataBindings.SelectedObjectId = value.Id;
    //            DataBindings.SelectedObjectName = value.Name;
    //        }
    //    } catch (Exception ex){
    //        _helpers.ProvideGeneralErrorFeedback(ex, "Encountered an unexpected problem when the selected object was changed.");
    //    }
    //}

    [RelayCommand] private void ClearObjectID() => _helpers.ClearSelectedObjectField(FieldsToClear.IdField);
    [RelayCommand] private void ClearObjectName() => _helpers.ClearSelectedObjectField(FieldsToClear.NameField);
    [RelayCommand] private void OpenAdminWindow() => ToggleAdminWindow();
    [RelayCommand] private void CloseAdminWindow() => ToggleAdminClose();
    [RelayCommand] private void CloseApplication() => RequestApplicationClose?.Invoke();
    [RelayCommand] private void InitialiseObjectAdd() => _manager.InitialiseDataInsertion(DataBindings.NewObjectName);
    [RelayCommand] private void InitialiseObjectDelete() => ConfigureDeletionObject();
    [RelayCommand] private void InitialiseObjectUpdate() => ConfigureUpdateObject();

    //private void ClearSelectedObjectField (ObjectFieldToClear fieldToClear){
    //    try {
    //        if (DataBindings.SelectedObject == null){
    //            UserFeedback.WarningFeedback("Failed to clear the selected field, no object was found");
    //        } else {
    //            switch (fieldToClear){
    //                case ObjectFieldToClear.IdField:
    //                    DataBindings.SelectedObjectId = null;
    //                    DataBindings.ShouldFocusObjectIdField = true;
    //                    break;
    //                case ObjectFieldToClear.NameField:
    //                    DataBindings.SelectedObjectName = string.Empty;
    //                    DataBindings.ShouldFocusObjectNameField = true;
    //                    break;
    //                default:
    //                    UserFeedback.WarningFeedback($"Unexpected StaffField value: {fieldToClear}. No action taken.");
    //                    break;
    //            }
    //        }
    //    } catch (Exception ex){
    //        UserFeedback.ErrorFeedback(ex, "Unable to clear the selected object field, something went wrong.");
    //    }
    //}

    private void ToggleAdminWindow(){
        try {
            if (DataBindings.SelectedObject != null){
                DataBindings.UpdatedObjectName = DataBindings.SelectedObjectName;
            }

            RequestNewWindow?.Invoke();
        } catch (Exception ex){
            _helpers.ProvideGeneralErrorFeedback(ex, "Failed to Initialise the Admin Window.");
        }
    }

    private void ToggleAdminClose(){
        try {
            _helpers.UpdateObjectFieldsPostSave(_manager.InitialiseDataSave());
            RequestWindowClose?.Invoke();
        } catch (Exception ex){
            _helpers.ProvideGeneralErrorFeedback(ex, "Failed to close the admin window something went wrong.");
        }
    }

    //private void UpdateObjectFieldsPostSave (bool success){
    //    try {
    //        if (success){
    //            UserFeedback.InformationFeedback("Successfully saved the changes to the data");
    //        } else {
    //            UserFeedback.WarningFeedback("Failed to save the changes to the data something went wrong");
    //        }
            
    //        DataBindings.SelectedObject = new();
    //        DataBindings.SearchTerm = string.Empty;
    //        DataBindings.UpdatedObjectName = string.Empty;
    //        DataBindings.NewObjectName = string.Empty;
    //    } catch (Exception ex){
    //        UserFeedback.ErrorFeedback(ex, "Encountered unexpected problem updating the object fields.");
    //    }
    //}

    private void ConfigureDeletionObject (){
        try {
            if(_manager.InitialiseDataDeletion(DataBindings.SelectedObject.Id!.Value)){
               _helpers.ProvideGeneralFeedback($"Successfully deleted the employee matching ID: {DataBindings.SelectedObject.Id!.Value}");
            } else {
                _helpers.ProvideGeneralWarningFeedback($"Failed to delete the employee matching ID: {DataBindings.SelectedObject.Id!.Value}, something went wrong");
            }

            //  Trying this over resetting the individual properties of the current object.
            //DataBindings.SelectedObject = new();
            //DataBindings.UpdatedObjectName = string.Empty;

            _helpers.UpdateDeletionObjectFields();
        } catch (Exception ex){
            _helpers.ProvideGeneralErrorFeedback(ex, "Encountered unexpected problem attempting to configure the object for deletion.");
        }
    }

    private void ConfigureUpdateObject (){
        if (_manager.InitialiseDataUpdate(DataBindings.SelectedObjectId, DataBindings.UpdatedObjectName)){
            _helpers.ProvideGeneralFeedback($"Successfully updated the employees record for ID: {DataBindings.SelectedObjectId}.");
        } else {
            _helpers.ProvideGeneralWarningFeedback($"Failed to update the employees record for ID: {DataBindings.SelectedObjectId}. something went wrong.");
        }
    }
}