using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StaffManager.DataModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace StaffManager.Classes;

public partial class OrdinaryDictionaryManager: ObservableObject {
    public StaffData StaffData { get; } = new StaffData();
    public event Action? RequestNewWindow;
    public event Action? RequestClose;
    public event Action? RequestApplicationClose;

    //  This constructor validates the sorted staff data to ensure it can be loaded and subscribes to the
    //  PropertyChanged event of StaffData to react to future data changes.
    public OrdinaryDictionaryManager (){
        Stopwatch sw = Stopwatch.StartNew();
        DataValidator.ValidateLoadableData(StaffData.MasterFile.Data!);
        sw.Stop();

        UserFeedback.LogApplicationTime($"Time taken to load data in Dictionary<int, string>: {sw.ElapsedMilliseconds} ms");

        StaffData.PropertyChanged += StaffDataPropertyChanged;
    }

    //  This method responds to changes in specific properties of StaffData, filtering staff members when the
    //  search term changes and triggering an update when the selected staff member changes.
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

    //  This method updates the selected staff name and ID in StaffData when a new staff member is selected,
    //  provided the selected value is not null.
    private void OnSelectedStaffMemberChanged (StaffMember? value){
        if (value != null){
            StaffData.SelectedStaffName = value.Name;
            StaffData.SelectedStaffId = value.Id;
        }
    }

    //  This method filters the staff list based on a search term, matching either staff IDs or names depending on the input,
    //  and updates the displayed list accordingly. If no data is available or an error occurs during filtering, it shows an
    //  appropriate error message to the user.
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

    //  This method clears either the name or ID field of the currently selected staff member based on the specified field,
    //  setting focus to the corresponding input field. If no staff member is selected or an unexpected field value is provided,
    //  it displays an appropriate error or warning message.
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

    //  This command method clears the selected staff member's ID by calling ClearSelectedStaffField with the Id option,
    //  typically triggered by a UI action bound to the command.
    [RelayCommand] private void ClearSelectedId () => ClearSelectedStaffField(StaffFieldToClear.Id);
    
    //  This command method clears the selected staff member's name by invoking ClearSelectedStaffField with the Name option,
    //  allowing the UI to trigger this action through data binding.
    [RelayCommand] private void ClearSelectedName () => ClearSelectedStaffField(StaffFieldToClear.Name);
    
    //  This command method updates the UpdatedStaffName with the currently selected staff member’s name and then
    //  triggers the RequestNewWindow event to open a new window.
    [RelayCommand] private void OpenNewWindow (){ 
        if (StaffData.SelectedStaffMember != null){
            StaffData.UpdatedStaffName = StaffData.SelectedStaffMember.Name;
        }
        RequestNewWindow?.Invoke(); 
    }
    
    //  This command method validates the staff data before saving, triggers the window close request,
    //  clears the selected staff ID and name fields, and resets the search term to empty.
    [RelayCommand] private void CloseWindow (){
        DataValidator.ValidateDataForSave(StaffData.MasterFile.Data!);        
        RequestClose?.Invoke();
        UpdatedStaffDataFields();
    }

    private void UpdatedStaffDataFields (){
        StaffData.SelectedStaffMember = new();
        StaffData.SearchTerm = string.Empty;
        StaffData.UpdatedStaffName = string.Empty;
        StaffData.NewStaffName = string.Empty;
    }
   
    //  This command method triggers the request to close the entire application when invoked.
    [RelayCommand] private void CloseApplication () => RequestApplicationClose?.Invoke();
    
    //  This command method validates the data for adding a new staff member using the current sorted data and the new staff name provided.
    [RelayCommand] private void AddNewStaffMember () => DataValidator.ValidateNewUserData(StaffData.MasterFile.Data!, StaffData.NewStaffName!);
    
    //  This command method validates the updated staff information by checking the current sorted data,
    //  the updated staff name, and the selected staff ID before applying changes.
    [RelayCommand] private void UpdateStaffRecord () => DataValidator.ValidateUpdateData(StaffData.MasterFile.Data!, StaffData.UpdatedStaffName!, StaffData.SelectedStaffId);
    
    //  This command method validates the deletion of a staff record using the selected staff ID,
    //  then clears the ID and name of the selected staff member and resets the updated staff name.
    [RelayCommand] private void DeleteRecord (){
        DataValidator.ValidateDeleteData(StaffData.MasterFile.Data!, StaffData.SelectedStaffId);
        StaffData.SelectedStaffMember.Id = null;
        StaffData.SelectedStaffMember.Name = string.Empty;
        StaffData.UpdatedStaffName = string.Empty;
    }
}