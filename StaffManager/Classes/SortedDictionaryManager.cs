using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErrorLogging;
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
                LoggingManager.Instance.LogWarning("Failed to initialise the data.");
            }

            StaffData.PropertyChanged += StaffDataPropertyChanged;
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to correctly initialise the SortedDictionaryManager!");
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
            default:
                LoggingManager.Instance.LogWarning($"Unhandled property change: {e.PropertyName}");
                break;
        }
    }

    private void OnSelectedStaffMemberChanged (StaffMember? value){
        if (value != null){
            StaffData.SelectedStaffName = value.Name;
            StaffData.SelectedStaffId = value.Id;
        } else {
            LoggingManager.Instance.LogWarning("No staff member was selected; cannot update name or ID fields.");
        }
    }

    private void FilterStaffMembers (string? searchTerm){
        try {
            if (StaffData.MasterFile.SortedData == null){
                LoggingManager.Instance.LogWarning("Filter failed: MasterFile.SortedData is null.");
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
            LoggingManager.Instance.LogError(ex, "An error occurred while filtering staff data.");
        }
    }

    private void ClearSelectedStaffField (StaffFieldToClear fieldToClear){
        if (StaffData.SelectedStaffMember == null){
            LoggingManager.Instance.LogWarning("Attempted to clear staff field, but no staff member is selected.");
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
                LoggingManager.Instance.LogWarning($"Unexpected StaffField value: {fieldToClear}. No action taken.");
                break;
        }
    }

    [RelayCommand] private void ClearSelectedId () => ClearSelectedStaffField(StaffFieldToClear.Id);
    [RelayCommand] private void ClearSelectedName () => ClearSelectedStaffField(StaffFieldToClear.Name);
    [RelayCommand] private void OpenNewWindow () => RequestNewWindow?.Invoke();
    [RelayCommand] private void CloseWindow () => RequestClose?.Invoke();
    [RelayCommand] private void CloseApplication () => RequestApplicationClose?.Invoke();
}