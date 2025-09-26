using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StaffManager.DataModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;

namespace StaffManager.Classes;

public partial class SharedUiManager: ObservableObject {
    public StaffData StaffData { get; } = new StaffData();
    public event Action? RequestNewWindow;
    public event Action? RequestClose;
    public event Action? RequestApplicationClose;

    [ObservableProperty] private IDictionary<int, string>? _masterFile;
    private enum StaffFieldToClear {
        Name,
        Id
    }

    //  This constructor validates the sorted staff data to ensure it can be loaded and subscribes to the
    //  PropertyChanged event of StaffData to react to future data changes.
    public SharedUiManager (IDictionary<int, string> theDictionary){
        try {
            //InitialiseDictionaryType(sortData);

            MasterFile = theDictionary;

            Stopwatch sw = Stopwatch.StartNew();
            MasterFile = DataValidator.ValidateLoadableData(MasterFile!);
            sw.Stop();

            UserFeedback.LogApplicationInformation($"Time taken to load data in Dictionary<int, string>: {sw.ElapsedMilliseconds} ms");

            StaffData.PropertyChanged += StaffDataPropertyChanged;
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Failed to create the Data processor for the ordinary dictionary", "Application Failure", ex);
        }
    }

    //  This method responds to changes in specific properties of StaffData, filtering staff members when the
    //  search term changes and triggering an update when the selected staff member changes.
    private void StaffDataPropertyChanged (object? sender, PropertyChangedEventArgs e){
        try {
            switch (e.PropertyName){
                case nameof(StaffData.SearchTerm):
                    FilterStaffMembers(StaffData.SearchTerm);
                    break;
                case nameof(StaffData.SelectedStaffMember):
                    OnSelectedStaffMemberChanged(StaffData.SelectedStaffMember);
                    break;
            }
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Encnountered an unexpected property change request.", "Invalid Property Change", ex);
        }
    }

    //  This method updates the selected staff name and ID in StaffData when a new staff member is selected,
    //  provided the selected value is not null.
    private void OnSelectedStaffMemberChanged (StaffMember? value){
        try {
            if (value != null){
                StaffData.SelectedStaffName = value.Name;
                StaffData.SelectedStaffId = value.Id;
            }
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Encountered an unexpected issue when selecting a staff memeber", "Selection Changed Error", ex);
        }
    }

    //  This method filters the staff list based on a search term, matching either staff IDs or names depending on the input,
    //  and updates the displayed list accordingly. If no data is available or an error occurs during filtering, it shows an
    //  appropriate error message to the user.
    private void FilterStaffMembers (string? searchTerm){
        try {
            if (MasterFile == null) {
                UserFeedback.DisplayErrorMessage("Filter failed: MasterFile.Data is null.", "No Data Error");
                return;
            }

            searchTerm = searchTerm?.Trim() ?? string.Empty;
            IEnumerable<StaffMember> filtered;

            if (string.IsNullOrWhiteSpace(searchTerm)){
                filtered = [];
            } else if (int.TryParse(searchTerm, out _)){
                filtered = MasterFile.Where(kvp => kvp.Key.ToString().StartsWith(searchTerm)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
            } else {
                filtered = MasterFile.Where(kvp => kvp.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
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
        try {
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
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Problem encountered updating the Ui while clearing fields.", "Data Clear Error", ex);
        }
    }

    //  This command method clears the selected staff member's ID by calling ClearSelectedStaffField with the Id option,
    //  typically triggered by a UI action bound to the command.
    [RelayCommand] private void ClearSelectedId() => ClearSelectedStaffField(StaffFieldToClear.Id);

    //  This command method clears the selected staff member's name by invoking ClearSelectedStaffField with the Name option,
    //  allowing the UI to trigger this action through data binding.
    [RelayCommand] private void ClearSelectedName() => ClearSelectedStaffField(StaffFieldToClear.Name);

    //  This command method updates the UpdatedStaffName with the currently selected staff member’s name and then
    //  triggers the RequestNewWindow event to open a new window.
    [RelayCommand]
    private void OpenNewWindow (){
        try {
            if (StaffData.SelectedStaffMember != null){
                StaffData.UpdatedStaffName = StaffData.SelectedStaffMember.Name;
            }
            RequestNewWindow?.Invoke();
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Failed to create the Admin Window", "UI Error", ex);
        }
    }

    //  This command method validates the staff data before saving, triggers the window close request,
    //  clears the selected staff ID and name fields, and resets the search term to empty.
    [RelayCommand]
    private void CloseWindow (){
        try {
            Stopwatch sw = Stopwatch.StartNew();
            DataValidator.ValidateDataForSave(MasterFile!);
            sw.Stop();

            UserFeedback.LogApplicationInformation($"Time taken to save data in Dictionary<int, string>: {sw.ElapsedMilliseconds} ms");

            RequestClose?.Invoke();
            UpdatedStaffDataFields();
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Encountered an error closing the AdminWindow", "UI Error", ex);
        }
    }

    //  This method is used to update the imput fields after the Admin panel has been closed.
    private void UpdatedStaffDataFields (){
        try {
            StaffData.SelectedStaffMember = new();
            StaffData.SearchTerm = string.Empty;
            StaffData.UpdatedStaffName = string.Empty;
            StaffData.NewStaffName = string.Empty;
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Encountered unexpected problem, updating the input fields", "UI Field Error", ex);
        }
    }

    //  This command method triggers the request to close the entire application when invoked.
    [RelayCommand] private void CloseApplication() => RequestApplicationClose?.Invoke();

    //  This command method validates the data for adding a new staff member using the current
    //  sorted data and the new staff name provided.
    [RelayCommand] private void AddNewStaffMember(){
        DataValidator.ValidateNewUserData(MasterFile!, StaffData.NewStaffName!);
    }

    //  This command method validates the updated staff information by checking the current sorted data,
    //  the updated staff name, and the selected staff ID before applying changes.
    [RelayCommand] private void UpdateStaffRecord() => DataValidator.ValidateUpdateData(MasterFile!, StaffData.UpdatedStaffName!, StaffData.SelectedStaffId);

    //  This command method validates the deletion of a staff record using the selected staff ID,
    //  then clears the ID and name of the selected staff member and resets the updated staff name.
    [RelayCommand]
    private void DeleteRecord (){
        try {
            DataValidator.ValidateDeleteData(MasterFile!, StaffData.SelectedStaffId);
            StaffData.SelectedStaffMember.Id = null;
            StaffData.SelectedStaffMember.Name = string.Empty;
            StaffData.UpdatedStaffName = string.Empty;
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Encountered unexpected problem, attempting to delete the user details", "Data Update Error", ex);
        }
    }
}