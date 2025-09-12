namespace StaffManager.DataModels;

public class MasterFile {
    public Dictionary<int, string> OrdinaryDictionary { get; set; } = [];
    public SortedDictionary<int, string> SortedDictionary { get; set; } = [];
}