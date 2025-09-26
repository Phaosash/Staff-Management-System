using StaffManager.Classes;
using System.Windows;

namespace StaffManager.SharedUi;

public partial class AdminPanel : Window {
    //  This constructor initializes the AdminPanel user interface and sets its data context to an instance of SortedDictionaryManager.
    //  It also subscribes to the RequestClose event of the manager, closing the panel when that event is triggered.
    public AdminPanel (SharedUiManager sharedUiManager){
        InitializeComponent();

        DataContext = sharedUiManager;
        sharedUiManager.RequestClose += () => this.Close();
    }
}
