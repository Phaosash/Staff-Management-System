using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;

namespace StaffManager.Classes;

internal class FileManager {
    //  This method reads a CSV file from a specified path and loads its data into a dictionary, mapping integer keys to string values.
    //  If the file doesn't exist, an error message is shown, and a warning is logged. It validates each line for errors like missing or
    //  invalid data, non-integer keys, and duplicate keys, providing appropriate error messages for each issue. If an error occurs while
    //  reading or parsing the CSV, it catches the exception, displays a message, and logs the error.
    public static void LoadFromCsv (string filePath, IDictionary<int, string> dictionary){
        if (!File.Exists(filePath)){
            UserFeedback.DisplayErrorMessage($"The specified file doesn't exist in {filePath}.", "File Path Error");
            return;
        }

        try {
            using var reader = new StreamReader(filePath);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture){ HasHeaderRecord = false, IgnoreBlankLines = true, BadDataFound = context => {
                if (context.Context != null){
                    var csvContext = context.Context;
                    int currentRow = csvContext.Parser!.Row;
                    UserFeedback.DisplayErrorMessage($"Line {currentRow}: Bad data in CSV: {context.RawRecord}", "CSV Error");
                } else {
                    UserFeedback.DisplayErrorMessage($"Bad data in CSV: {context.RawRecord}", "CSV Error");
                }
            } };

            using var csv = new CsvReader(reader, config);

            int lineNumber = 0;
            while (csv.Read()){
                lineNumber++;

                try {
                    var keyStr = csv.GetField(0)?.Trim();
                    var value = csv.GetField(1)?.Trim();

                    if (string.IsNullOrWhiteSpace(keyStr) || string.IsNullOrWhiteSpace(value)){
                        UserFeedback.DisplayErrorMessage($"One or more fields are empty on Line: {lineNumber}", "CSV Value Error");
                        continue;
                    }

                    if (!int.TryParse(keyStr, out int key)){
                        UserFeedback.DisplayErrorMessage($"Line {lineNumber}: Invalid key '{keyStr}' — must be an integer.", "Key Value Error");
                        continue;
                    }

                    if (dictionary.ContainsKey(key)){
                        UserFeedback.DisplayErrorMessage($"Line {{lineNumber}}: Duplicate key found.", "Duplicate Key Value Error");
                        continue;
                    }

                    dictionary[key] = value;
                } catch (Exception ex){
                    UserFeedback.DisplayErrorMessageWithException($"Error parsing CSV on Line {lineNumber}: ", "CSV Error", ex);
                }
            }
        } catch (Exception ex){
            UserFeedback.DisplayErrorMessageWithException($"Could not read the CSV file.", "File Read Error", ex);
        }
    }

    //  This method writes the contents of a dictionary to a CSV file at the specified path. It iterates over the dictionary,
    //  writing each key-value pair as a row in the CSV file. If the file is saved successfully, a success message is displayed,
    //  and an informational log is recorded. If an error occurs during the file writing process, an error message is shown, and
    //  the exception is logged.
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