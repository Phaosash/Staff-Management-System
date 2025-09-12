using CsvHelper;
using CsvHelper.Configuration;
using ErrorLogging;
using System.Globalization;
using System.IO;
using System.Windows;

namespace StaffManager.Classes;

public class FileReader {
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
}