using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StaffManager.DataModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StaffManager.Classes;

public partial class OrdinaryDictionaryManager: ObservableObject {
    public StaffData StaffData { get; } = new StaffData();
    public event Action? RequestNewWindow;
    public event Action? RequestClose;
    public event Action? RequestApplicationClose;

    public OrdinaryDictionaryManager (){
        DataValidator.ValidateLoadableData(StaffData.MasterFile.Data!);
        StaffData.PropertyChanged += StaffDataPropertyChanged;
    }

    private void StaffDataPropertyChanged (object? sender, PropertyChangedEventArgs e){
        switch (e.PropertyName){
            case nameof(StaffData.SearchTerm):
                FilterStaffMembers(StaffData.SearchTerm);
                break;
            case nameof(StaffData.SelectedStaffMember):
                OnSelectedStaffMemberChanged(StaffData.SelectedStaffMember);
                break;
        }
    }

    private void OnSelectedStaffMemberChanged (StaffMember? value){
        if (value != null){
            StaffData.SelectedStaffName = value.Name;
            StaffData.SelectedStaffId = value.Id;
        }
    }

    private void FilterStaffMembers (string? searchTerm){
        try {
            if (StaffData.MasterFile.Data == null){
                UserFeedback.DisplayErrorMessage("Filter failed: MasterFile.Data is null.", "No Data Error");
                return;
            }

            searchTerm = searchTerm?.Trim() ?? string.Empty;
            IEnumerable<StaffMember> filtered;

            if (string.IsNullOrWhiteSpace(searchTerm)){
                filtered = [];
            } else if (int.TryParse(searchTerm, out _)){
                filtered = StaffData.MasterFile.Data.Where(kvp => kvp.Key.ToString().StartsWith(searchTerm)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
            } else {
                filtered = StaffData.MasterFile.Data.Where(kvp => kvp.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
            }

            StaffData.StaffMembers = new ObservableCollection<StaffMember>(filtered);
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("An error occurred while filtering staff data.", "Data Filter Error", ex);
        }
    }

    private void ClearSelectedStaffField (StaffFieldToClear fieldToClear){
        if (StaffData.SelectedStaffMember == null){
            UserFeedback.DisplayErrorMessage("Attempted to clear staff field, but no staff member is selected.", "No Data Error");
            return;
        }

        switch (fieldToClear){
            case StaffFieldToClear.Name:
                StaffData.SelectedStaffName = string.Empty;
                StaffData.ShouldFocusNameTextBox = true;
                break;
            case StaffFieldToClear.Id:
                StaffData.SelectedStaffId = null;
                StaffData.ShouldFocusIdTextBox = true;
                break;
            default:
                UserFeedback.DisplayWarning($"Unexpected StaffField value: {fieldToClear}. No action taken.", "Unexpected Value");
                break;
        }
    }

    [RelayCommand] private void ClearSelectedId () => ClearSelectedStaffField(StaffFieldToClear.Id);
    [RelayCommand] private void ClearSelectedName () => ClearSelectedStaffField(StaffFieldToClear.Name);
    [RelayCommand] private void OpenNewWindow (){ 
        StaffData.UpdatedStaffName = StaffData.SelectedStaffMember.Name;
        RequestNewWindow?.Invoke(); 
    }
    [RelayCommand] private void CloseWindow (){
        DataValidator.ValidateDataForSave(StaffData.MasterFile.Data!);        
        RequestClose?.Invoke();
        ClearSelectedId();
        ClearSelectedName();
        StaffData.SearchTerm = string.Empty;
    }
    [RelayCommand] private void CloseApplication () => RequestApplicationClose?.Invoke();
    [RelayCommand] private void AddNewStaffMember () => DataValidator.ValidateNewUserData(StaffData.MasterFile.Data!, StaffData.NewStaffName!);
    [RelayCommand] private void UpdateStaffRecord () => DataValidator.ValidateUpdateData(StaffData.MasterFile.Data!, StaffData.UpdatedStaffName!, StaffData.SelectedStaffId);
    [RelayCommand] private void DeleteRecord (){
        DataValidator.ValidateDeleteData(StaffData.MasterFile.Data!, StaffData.SelectedStaffId);
        StaffData.SelectedStaffMember.Id = null;
        StaffData.SelectedStaffMember.Name = string.Empty;
        StaffData.UpdatedStaffName = string.Empty;
    }
}