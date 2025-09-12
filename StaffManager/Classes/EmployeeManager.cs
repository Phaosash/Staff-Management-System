using CommunityToolkit.Mvvm.ComponentModel;
using StaffManager.DataModels;
using System.IO;

namespace StaffManager.Classes;

public partial class EmployeeManager: ObservableObject {
    private MasterFile _masters = new();

    public EmployeeManager (){
        InitialiseData();
    }

    private void InitialiseData (){
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "MalinStaffNamesV3.csv");
        FileReader.LoadFromCsv(path, _masters.OrdinaryDictionary);
        FileReader.LoadFromCsv(path, _masters.SortedDictionary);
    }

    public Dictionary<int, string> GetOrdinaryDictionary (){
        return _masters.OrdinaryDictionary;
    }

    public SortedDictionary<int, string> GetSortedDictionary (){
        return _masters.SortedDictionary;
    }
}