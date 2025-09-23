using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace StaffManager.Classes;

internal class FileManager {
    //  This method loads key-value pairs from a CSV file into a dictionary, validating each line for missing or invalid data,
    //  duplicate keys, and reporting errors with detailed messages. It gracefully handles file absence and parsing exceptions,
    //  providing user feedback for any issues encountered during the process.
    public static void LoadFromCsv (string filePath, IDictionary<int, string> dictionary){
        if (!File.Exists(filePath)){
            UserFeedback.DisplayErrorMessage($"The specified file doesn't exist in {filePath}.", "File Path Error");
            return;
        }

        try {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, GetCsvConfiguration());

            int lineNumber = 0;
            while (csv.Read()){
                lineNumber++;
                ProcessCsvLine(csv, lineNumber, dictionary);
            }
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException($"Could not read the CSV file.", "File Read Error", ex);
        }
    }

    private static CsvConfiguration GetCsvConfiguration (){
        return new CsvConfiguration(CultureInfo.InvariantCulture){
            HasHeaderRecord = false,
            IgnoreBlankLines = true,
            BadDataFound = context => {
                int row = context.Context?.Parser?.Row ?? -1;
                string message = row > 0
                    ? $"Line {row}: Bad data in CSV: {context.RawRecord}"
                    : $"Bad data in CSV: {context.RawRecord}";
                UserFeedback.DisplayErrorMessage(message, "CSV Error");
            }
        };
    }

    private static void ProcessCsvLine (CsvReader csv, int lineNumber, IDictionary<int, string> dictionary) {
        try {
            var keyStr = csv.GetField(0)?.Trim();
            var value = csv.GetField(1)?.Trim();

            if (string.IsNullOrWhiteSpace(keyStr) || string.IsNullOrWhiteSpace(value)){
                UserFeedback.DisplayErrorMessage($"One or more fields are empty on Line: {lineNumber}", "CSV Value Error");
                return;
            }

            if (!int.TryParse(keyStr, out int key)){
                UserFeedback.DisplayErrorMessage($"Line {lineNumber}: Invalid key '{keyStr}' — must be an integer.", "Key Value Error");
                return;
            }

            if (dictionary.ContainsKey(key)){
                UserFeedback.DisplayErrorMessage($"Line {lineNumber}: Duplicate key found.", "Duplicate Key Value Error");
                return;
            }

            dictionary[key] = value;
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException($"Error parsing CSV on Line {lineNumber}.", "CSV Error", ex);
        }
    }

    //  This method saves key-value pairs from a dictionary to a CSV file without a header, writing each entry as a separate record,
    //  and provides user feedback on success or failure with detailed error information if an exception occurs.
    public static void SaveToCSV (string filePath, IDictionary<int, string> data){
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
            UserFeedback.DisplayInformation($"Successfully saved the the changes to the data.", "Success");
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException("Unable to save file, something went wrong!", "File Save Error", ex);
        }
    }
}