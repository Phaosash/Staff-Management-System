using CsvHelper;
using CsvHelper.Configuration;
using ErrorLogging;
using System.Globalization;
using System.IO;
using System.Windows;

namespace StaffManager.Classes;

internal class FileManager {
    //  This method reads a CSV file from a specified path and loads its data into a dictionary, mapping integer keys to string values.
    //  If the file doesn't exist, an error message is shown, and a warning is logged. It validates each line for errors like missing or
    //  invalid data, non-integer keys, and duplicate keys, providing appropriate error messages for each issue. If an error occurs while
    //  reading or parsing the CSV, it catches the exception, displays a message, and logs the error.
    public static void LoadFromCsv (string filePath, IDictionary<int, string> dictionary){
        if (!File.Exists(filePath)){
            MessageBox.Show($"The specified file doesn't exist in {filePath}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            LoggingManager.Instance.LogWarning($"The specified file doesn't exist in {filePath}.");
            return;
        }

        try {
            using var reader = new StreamReader(filePath);
            var config = new CsvConfiguration(CultureInfo.InvariantCulture){
                HasHeaderRecord = false,
                IgnoreBlankLines = true,
                BadDataFound = context => {
                    if (context.Context != null){
                        var csvContext = context.Context;
                        int currentRow = csvContext.Parser!.Row;
                        MessageBox.Show($"Line {currentRow}: Bad data in CSV: {context.RawRecord}", "CSV Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        LoggingManager.Instance.LogWarning($"Line {currentRow}: Bad data in CSV: {context.RawRecord}");
                    } else {
                        MessageBox.Show($"Bad data in CSV: {context.RawRecord}", "CSV Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        LoggingManager.Instance.LogWarning($"Bad data in CSV: {context.RawRecord}");
                    }
                }
            };

            using var csv = new CsvReader(reader, config);

            int lineNumber = 0;
            while (csv.Read()){
                lineNumber++;

                try {
                    var keyStr = csv.GetField(0)?.Trim();
                    var value = csv.GetField(1)?.Trim();

                    if (string.IsNullOrWhiteSpace(keyStr) || string.IsNullOrWhiteSpace(value)){
                        MessageBox.Show($"Line {lineNumber}: One or more fields are empty.", "CSV Value Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        LoggingManager.Instance.LogWarning($"Line {lineNumber}: Empty key or value.");
                        continue;
                    }

                    if (!int.TryParse(keyStr, out int key)){
                        MessageBox.Show($"Line {lineNumber}: Invalid key '{keyStr}' — must be an integer.", "Key Value Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        LoggingManager.Instance.LogWarning($"Line {lineNumber}: Invalid key '{keyStr}' — must be an integer.");
                        continue;
                    }

                    if (dictionary.ContainsKey(key)){
                        MessageBox.Show($"Line {lineNumber}: Duplicate key found: {key}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        LoggingManager.Instance.LogWarning($"Line {lineNumber}: Duplicate key found.");
                        continue;
                    }

                    dictionary[key] = value;
                } catch (Exception ex){
                    MessageBox.Show($"Line {lineNumber}: Error parsing CSV: {ex.Message}", "CSV Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    LoggingManager.Instance.LogError(ex, $"Error parsing CSV on Line {lineNumber}: ");
                }
            }
        } catch (Exception ex){
            MessageBox.Show($"Could not read file: {ex.Message}", "File Read Error", MessageBoxButton.OK, MessageBoxImage.Error);
            LoggingManager.Instance.LogError(ex, $"Exception reading CSV file: {ex}");
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

            MessageBox.Show($"Dictionary saved to {filePath}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoggingManager.Instance.LogInformation($"Dictionary saved to {filePath}");
        } catch (Exception ex){
            MessageBox.Show($"Could not save file: {ex.Message}", "File Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            LoggingManager.Instance.LogError(ex, $"Exception saving CSV file: {ex}");
        }
    }
}