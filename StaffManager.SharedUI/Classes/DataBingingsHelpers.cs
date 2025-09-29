using CommunityToolkit.Mvvm.ComponentModel;
using StaffManager.Manager.ErrorHandling;
using StaffManager.SharedUI.DataModels;

namespace StaffManager.SharedUI.Classes;

internal partial class DataBingingsHelpers (BindingObject bindingObject): ObservableObject{
    private readonly BindingObject _bindings = bindingObject;
    
    public void ClearSelectedObjectField (FieldsToClear fieldToClear){
        try {
            if (_bindings.SelectedObject == null){
                UserFeedback.WarningFeedback("Failed to clear the selected field, no object was found");
            } else {
                switch (fieldToClear){
                    case FieldsToClear.IdField:
                        _bindings.SelectedObjectId = null;
                        _bindings.ShouldFocusObjectIdField = true;
                        break;
                    case FieldsToClear.NameField:
                        _bindings.SelectedObjectName = string.Empty;
                        _bindings.ShouldFocusObjectNameField = true;
                        break;
                    default:
                        UserFeedback.WarningFeedback($"Unexpected StaffField value: {fieldToClear}. No action taken.");
                        break;
                }
            }
        } catch (Exception ex){
            ProvideGeneralErrorFeedback(ex, "Unable to clear the selected object field, something went wrong.");
        }
    }

    public void UpdateObjectFieldsPostSave (bool success){
        try {
            if (success){
                ProvideGeneralFeedback("Successfully saved the changes to the data");
            } else {
                ProvideGeneralWarningFeedback("Failed to save the changes to the data something went wrong");
            }
            
            _bindings.SelectedObject = new();
            _bindings.SearchTerm = string.Empty;
            _bindings.UpdatedObjectName = string.Empty;
            _bindings.NewObjectName = string.Empty;
        } catch (Exception ex){
            ProvideGeneralErrorFeedback(ex, "Encountered unexpected problem updating the object fields.");
        }
    }

    public void UpdateDeletionObjectFields (){
        _bindings.SelectedObject = new();
        _bindings.UpdatedObjectName = string.Empty;
    }

    public void OnSelectedObjectChanged (SelectableObjects value){
        try {
            if (value != null){
                _bindings.SelectedObjectId = value.Id;
                _bindings.SelectedObjectName = value.Name;
            }
        } catch (Exception ex){
            ProvideGeneralErrorFeedback(ex, "Encountered an unexpected problem when the selected object was changed.");
        }
    }

    public void ProvideGeneralFeedback (string message){
        _bindings.GeneralFeedback = message;
        LoggingManager.Instance.LogInformation(message);
    }

    public void ProvideGeneralWarningFeedback (string message){
        _bindings.GeneralFeedback = message;
        LoggingManager.Instance.LogWarning(message);
    }

    public void ProvideGeneralErrorFeedback (Exception ex, string message){
        _bindings.GeneralFeedback = message;
        LoggingManager.Instance.LogError(ex, message);
    }

    public void ProvideAdminFeedback (string message){
        _bindings.AdminFeedback = message;
        LoggingManager.Instance.LogInformation(message);
    }

    public void ProvideAdminWarningFeedback (string message){
        _bindings.AdminFeedback = message;
        LoggingManager.Instance.LogWarning(message);
    }

    public void ProvideAdminErrorFeedback (Exception ex, string message){
        _bindings.AdminFeedback = message;
        LoggingManager.Instance.LogError(ex, message);
    }
}