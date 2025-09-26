using CsvHelper;
using CsvHelper.Configuration;
using StaffManager.Manager.ErrorHandling;
using System.Globalization;

namespace StaffManager.Manager.Classes;

internal class CsvFileManager {
    public static List<string> ReadGenericFile (string filePath){
        _ = new List<string>();
        List<string>? fileContents;
        
        try {
            fileContents = [.. File.ReadAllLines(filePath)];
        } catch (Exception ex){
            fileContents = [];
            LoggingManager.Instance.LogError(ex, "Failed to load the file, no file was found.");
        }

        return fileContents;
    }

    public static bool SaveIDictionaryToCSV (string filePath, IDictionary<int, string> data){
        try {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, new CsvConfiguration (CultureInfo.InvariantCulture){
                Delimiter = ",",
                HasHeaderRecord = false
            });

            foreach (var kvp in data){
                csv.WriteField(kvp.Key);
                csv.WriteField(kvp.Value);
                csv.NextRecord();
            }
            return true;

        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Unable to save file, something went wrong!");
            return false;
        }
    }
}