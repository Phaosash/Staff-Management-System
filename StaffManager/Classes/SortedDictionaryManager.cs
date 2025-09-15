using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ErrorLogging;
using StaffManager.DataModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace StaffManager.Classes;

public partial class SortedDictionaryManager: ObservableObject {
    [ObservableProperty] private SortedDictionary<int, string> _masterFile;
    [ObservableProperty] private ObservableCollection<StaffMember> _staffMembers;
    [ObservableProperty] private string? _searchTerm = string.Empty;

    public SortedDictionaryManager (){
        _masterFile = [];
        _staffMembers = [];

        DataManager.InitialiseData(_masterFile);
    }

    private bool CheckDataExists (){
        if (MasterFile == null){
            LoggingManager.Instance.LogWarning("MasterFile is null.");
            MessageBox.Show("Unable to search for the staff memeber the data is missing.", "No Data Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        if (string.IsNullOrEmpty(SearchTerm)){
            LoggingManager.Instance.LogWarning("SearchTerm is null.");
            MessageBox.Show("Unable to search for the staff member, invalid search term.", "Invalid Search Term", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }

        return true;
    }

    [RelayCommand]
    private void SearchForStaffMember (){
        try {
            if (CheckDataExists()){
                var staffMembers = DataManager.FindStaffMember(MasterFile, SearchTerm!); 

                if (staffMembers != null && staffMembers.Any()){
                    foreach (var staffMember in staffMembers){
                        StaffMembers.Add(staffMember);
                    }
                } else {
                    MessageBox.Show($"No recoord was found matching {SearchTerm}");
                    LoggingManager.Instance.LogWarning($"No record was found matching the search term: {SearchTerm}, staffMembers was null");
                }
            }
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, $"Encountered an unexpected problem while searching for a staff member, while using search term: {SearchTerm}.");
        }
    }

    //  This method is used to intialise the deletion of a staff members record from the SortedDictionary.
    //  It checks to ensure that data actually exists before trying to delete it.
    [RelayCommand] private void DeleteStaffMember (){ 
        if (CheckDataExists()){
            DataManager.DeleteEntryViaKey(MasterFile, SearchTerm!);
        }
    }

    [RelayCommand] private void AddStaffMember (){ 
        
    }

    //  This method is used to initialise the saving of any changes to the data back into the .csv file. If no data is found then it returns early as there
    //  will be nothing to save, and logs any issues that are encountered.
    [RelayCommand] private void SaveDictionary (){
        try {
            if (MasterFile == null){
                LoggingManager.Instance.LogWarning("MasterFile is null.");
                MessageBox.Show("Unable to save the updated data, no data was found.", "No Data Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            } else {
                DataManager.SaveDataToCsv(MasterFile);
            }
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Failed to initalise saving the file to .csv something went wrong!");
        }
    }
}