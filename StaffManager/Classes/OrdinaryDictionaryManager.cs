using CommunityToolkit.Mvvm.ComponentModel;
using ErrorLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StaffManager.Classes;

public partial class OrdinaryDictionaryManager: ObservableObject {
    private readonly Dictionary<int, string> _masterFile;

    public OrdinaryDictionaryManager (){
        _masterFile = [];

        DataManager.InitialiseData(_masterFile);
    }

    public Dictionary<int, string> GetMasterFile (){
        try {
            return _masterFile;
        } catch (Exception ex){
            LoggingManager.Instance.LogError(ex, "Unable to get the master file, master file is null.");
            MessageBox.Show("Failed to access the data something went wrong.", "Data access error", MessageBoxButton.OK, MessageBoxImage.Error);
            return [];
        }
    }
}