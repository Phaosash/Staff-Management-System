using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErrorLogging;
using StaffManager.DataModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace StaffManager.Classes;

public partial class SortedDictionaryManager: ObservableObject {
    public StaffData StaffData { get; } = new StaffData();
    public event Action? RequestNewWindow;
    public event Action? RequestClose;
    public event Action? RequestApplicationClose;

    public SortedDictionaryManager (){
        if (StaffData.MasterFile.SortedData != null){
            DataManager.InitialiseData(StaffData.MasterFile.SortedData);
        }

        StaffData.PropertyChanged += StaffDataPropertyChanged;
    }

    private void StaffDataPropertyChanged (object? sender, PropertyChangedEventArgs e){
        if (e.PropertyName == nameof(StaffData.SearchTerm)){
            FilterStaffMembers(StaffData.SearchTerm);
        }

        if (e.PropertyName == nameof(StaffData.SelectedStaffMember)){
            OnSelectedStaffMemberChanged(StaffData.SelectedStaffMember);
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
            searchTerm = searchTerm?.Trim() ?? "";
            IEnumerable<StaffMember> filtered;

            if (StaffData.MasterFile.SortedData == null){
                LoggingManager.Instance.LogWarning("Failed to filter the staff data no data was found.");
                return;
            }

            if (string.IsNullOrWhiteSpace(searchTerm)){
                filtered = [];
            } else if (int.TryParse(searchTerm, out _)){
                filtered = StaffData.MasterFile.SortedData.Where(kvp => kvp.Key.ToString().StartsWith(searchTerm)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
            } else {
                filtered = StaffData.MasterFile.SortedData.Where(kvp => kvp.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
            }

            StaffData.StaffMemebers = new ObservableCollection<StaffMember>(filtered);
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to Filter the staff data.");
        }
    }

    [RelayCommand] private void ClearId (){
        ClearSelectedField(false);
    }

    [RelayCommand] private void ClearName (){
        ClearSelectedField(true);
    }

    private void ClearSelectedField (bool clearName){
        if (StaffData.SelectedStaffMember != null){
            if (clearName){
                StaffData.SelectedStaffName = string.Empty;
                StaffData.ShouldFocusNameTextBox = true;
            } else {
                StaffData.SelectedStaffId = null;
                StaffData.ShouldFocusIdTextBox = true;
            }
        } else {
            LoggingManager.Instance.LogWarning("Failed to clear the selection field, no selected staff member was found.");
            MessageBox.Show("", "No Data Found Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand] private void OpenNewWindow (){
        RequestNewWindow?.Invoke();
    }

    [RelayCommand] private void CloseWindow (){
        RequestClose?.Invoke();
    }

    [RelayCommand] private void CloseApplication (){
        RequestApplicationClose?.Invoke();
    }
}