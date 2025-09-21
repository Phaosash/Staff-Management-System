using ErrorLogging;
using System.IO;

namespace StaffManager.Classes;

internal class DataManager {
    public static void InitialiseData (IDictionary<int, string> dictionary){
        try {
            var path = Path.Combine(AppContext.BaseDirectory, "Data", "MalinStaffNamesV3.csv");
            FileManager.LoadFromCsv(path, dictionary);
        } catch (Exception ex){ 
            LoggingManager.Instance.LogError(ex, "Failed to load the data, the file couldn't be found.");
        }
    }
}