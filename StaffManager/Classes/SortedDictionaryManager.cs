using CommunityToolkit.Mvvm.ComponentModel;
using ErrorLogging;
using StaffManager.DataModels;
using System.Collections.ObjectModel;

namespace StaffManager.Classes;

public partial class SortedDictionaryManager: ObservableObject {
    [ObservableProperty] private SortedDictionary<int, string> _masterFile = [];
    [ObservableProperty] private ObservableCollection<StaffMember> _staffMembers = [];
    [ObservableProperty] private string? _searchTerm = string.Empty;

    public SortedDictionaryManager (){
        DataManager.InitialiseData(_masterFile);
    }

    partial void OnSearchTermChanged (string? value){
        if (value != null){
            FilterStaffMembers(value);
        }
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
            LoggingManager.Instance.LogError(ex, "Failed to Filter the dictionary.");
        }
    }
}