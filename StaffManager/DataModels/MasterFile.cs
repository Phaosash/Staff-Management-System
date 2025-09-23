namespace StaffManager.DataModels;

//  This class contains two nullable properties: a dictionary and a sorted dictionary,
//  both mapping integers to strings, initialized as empty collections.
public class MasterFile {
    public Dictionary<int, string>? Data { get; set; } = [];
    public SortedDictionary<int, string>? SortedData { get; set; } = [];
}