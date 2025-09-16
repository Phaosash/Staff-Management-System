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
    [ObservableProperty] private string? _name = string.Empty;
    [ObservableProperty] private int _idNumber;
    [ObservableProperty] private string? _feedbackString = string.Empty;

    public SortedDictionaryManager (){
        _masterFile = [];
        _staffMembers = [];

        DataManager.InitialiseData(_masterFile);
    }

    [RelayCommand] private void SearchForStaffMember (){
        try {
            if (DataManager.CheckDataExists(MasterFile, SearchTerm!)){
                StaffMembers.Clear();

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
        if (DataManager.CheckDataExists(MasterFile, SearchTerm!)){
            DataManager.DeleteEntryViaKey(MasterFile, SearchTerm!);
        }
    }

    [RelayCommand] private void CreateStaffMember (){ 
        if (DataManager.ValidateStaffCreationFields(IdNumber, Name!)){
            var tempUser = new StaffMember();

            tempUser.Name = Name!;
            tempUser.Id = IdNumber;

            MasterFile.Add(tempUser.Id, tempUser.Name);
            StaffMembers.Add(tempUser);

            FeedbackString = "Successfully added the new staff member";
            LoggingManager.Instance.LogInformation($"Successfully add a new staff member ID: {tempUser.Id} Name: {tempUser.Name}");
        } else {
            FeedbackString = "Failed to add the new staff member.";
        }
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