using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErrorLogging;
using StaffManager.DataModels;
using System.Collections.ObjectModel;

namespace StaffManager.Classes;

public partial class SortedDictionaryManager: ObservableObject {
    public event Action? RequestNewWindow;
    public event Action? RequestClose;

    [ObservableProperty] private SortedDictionary<int, string> _masterFile = [];
    [ObservableProperty] private ObservableCollection<StaffMember> _staffMembers = [];
    [ObservableProperty] private string? _searchTerm = string.Empty;
    [ObservableProperty] private string? _searchStaffName = string.Empty;
    [ObservableProperty] private int _searchStaffId;
    [ObservableProperty] private StaffMember _selectedStaffMemeber = new();
    [ObservableProperty] private bool _shouldFocusIdTextBox;
    [ObservableProperty] private bool _shouldFocusNameTextBox;

    public SortedDictionaryManager (){
        DataManager.InitialiseData(_masterFile);
    }

    partial void OnSearchTermChanged (string? value){
        FilterStaffMembers(value);
    }

    private void FilterStaffMembers (string? searchTerm){
        try {
            searchTerm = searchTerm?.Trim() ?? "";
            IEnumerable<StaffMember> filtered;

            if (string.IsNullOrWhiteSpace(searchTerm)){
                filtered = [];
            } else if (int.TryParse(searchTerm, out _)){
                filtered = MasterFile.Where(kvp => kvp.Key.ToString().StartsWith(searchTerm)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
            } else {
                filtered = MasterFile.Where(kvp => kvp.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).Select(kvp => new StaffMember { Id = kvp.Key, Name = kvp.Value });
            }

            StaffMembers = new ObservableCollection<StaffMember>(filtered);
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to Filter the staff data.");
        }
    }

    [RelayCommand] private void ClearId (){
        if (SelectedStaffMemeber != null){
            SelectedStaffMemeber.Id = null;
            ShouldFocusIdTextBox = true;
        }
    }

    [RelayCommand] private void ClearName (){
        if (SelectedStaffMemeber != null){
            SelectedStaffMemeber.Name = string.Empty;
            ShouldFocusNameTextBox = true;
        }
    }

    [RelayCommand] private void OpenNewWindow (){
        RequestNewWindow?.Invoke();
    }

    [RelayCommand] private void CloseWindow (){
        RequestClose?.Invoke();
    }
}