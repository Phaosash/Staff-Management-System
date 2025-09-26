using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace StaffManager.Classes;

internal class FileManager {
    //  This method reads the content of a file and stores it in a List<string>
    public static List<string> ReadGenericFile (string filePath){
        _ = new List<string>();
        List<string>? fileContents;
        
        try {
            fileContents = [.. File.ReadAllLines(filePath)];
        } catch (Exception ex){
            fileContents = [];
            UserFeedback.DisplayErrorMessageWithException("Failed to load the file, no file was found.", "File Load Error", ex);
        }

        return fileContents;
    }

    //  This method saves key-value pairs from a dictionary to a CSV file without a header, writing each entry as a separate record,
    //  and provides user feedback on success or failure with detailed error information if an exception occurs.
    //  The save time is logged as a means of tracking the applications performance.
    public static void SaveIDictionaryToCSV (string filePath, IDictionary<int, string> data){
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
            //  Removing this has a dramatic effect on the applications write times, when saving the data back into the file
            //  UserFeedback.DisplayInformation($"Successfully saved the the changes to the data.", "Success");

            UserFeedback.LogApplicationInformation("Successfully saved the the changes to the data.");
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Unable to save file, something went wrong!", "File Save Error", ex);
        }
    }
}