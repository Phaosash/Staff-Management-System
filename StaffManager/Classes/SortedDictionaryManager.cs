using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StaffManager.DataModels;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StaffManager.Classes;

public partial class SortedDictionaryManager: ObservableObject {
    public StaffData StaffData { get; } = new StaffData();
    public event Action? RequestNewWindow;
    public event Action? RequestClose;
    public event Action? RequestApplicationClose;

    private enum StaffFieldToClear { Name, Id }

    public SortedDictionaryManager (){
        try {
            if (StaffData.MasterFile.SortedData != null){
                DataManager.InitialiseData(StaffData.MasterFile.SortedData);
            } else {
                UserFeedback.DisplayErrorMessage("Failed to initialise the data", "Data Initialisation Error");
            }

            StaffData.PropertyChanged += StaffDataPropertyChanged;
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Failed to correctly initialise the SortedDictionaryManager!", "Data Initalisation Error", ex);
        }
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
            if (StaffData.MasterFile.SortedData == null){
                UserFeedback.DisplayErrorMessage("Filter failed: MasterFile.SortedData is null.", "No Data Error");
                return;
            }

            searchTerm = searchTerm?.Trim() ?? string.Empty;
            IEnumerable<StaffMember> filtered;

            if (string.IsNullOrWhiteSpace(searchTerm)){
                filtered = [];
            } else if (int.TryParse(searchTerm, out _)){
                filtered = StaffData.MasterFile.SortedData.Where(kvp => kvp.Key.ToString().StartsWith(searchTerm)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
            } else {
                filtered = StaffData.MasterFile.SortedData.Where(kvp => kvp.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
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

    private void ModifyStaffRecord (DataOperations operation){
        if (StaffData.MasterFile.SortedData == null){
            UserFeedback.DisplayErrorMessage("Unable to update the record, no data was found.", "No Data Warning");
            return;
        }

        switch (operation){
            case DataOperations.Create:
                if (string.IsNullOrWhiteSpace(StaffData.NewStaffName)){
                    UserFeedback.DisplayErrorMessage("Unable to add new staff member no data was found.", "No Data Value");
                    return;
                } 

                DataManager.AddStaffMemberToIDictionary(StaffData.MasterFile.SortedData, StaffData.NewStaffName!);
                break;
            case DataOperations.Update:
                if (string.IsNullOrWhiteSpace(StaffData.UpdatedStaffName) || !StaffData.SelectedStaffId.HasValue){
                    UserFeedback.DisplayErrorMessage("Unable to update the record, no data has been found.", "No Data Warning");
                    return;
                }

                DataManager.UpdateStaffMembersName(StaffData.MasterFile.SortedData, StaffData.SelectedStaffId.Value, StaffData.UpdatedStaffName);
                break;
            case DataOperations.Delete:
                if (!StaffData.SelectedStaffId.HasValue){
                    return;
                }

                DataManager.DeleteRecord(StaffData.MasterFile.SortedData, (int)StaffData.SelectedStaffId);
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
        RequestClose?.Invoke();
        ClearSelectedId();
        ClearSelectedName();
    }
    [RelayCommand] private void CloseApplication () => RequestApplicationClose?.Invoke();
    [RelayCommand] private void AddNewStaffMember () => ModifyStaffRecord(DataOperations.Create);
    [RelayCommand] private void UpdateStaffRecord () => ModifyStaffRecord(DataOperations.Update);
    [RelayCommand] private void DeleteRecord () => ModifyStaffRecord(DataOperations.Delete);
}