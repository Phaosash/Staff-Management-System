namespace StaffManager.DataModels;

public class MasterFile {
    public Dictionary<int, string>? Data { get; set; }
    public SortedDictionary<int, string>? SortedData { get; set; }
}