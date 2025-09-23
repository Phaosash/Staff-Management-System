using StaffManager.Classes;
using System.Windows;

namespace SortedDictionaryApp.Views;

public partial class AdminPanel : Window {
    //  This constructor initializes the AdminPanel user interface and sets its data context to an instance of SortedDictionaryManager.
    //  It also subscribes to the RequestClose event of the manager, closing the panel when that event is triggered.
    public AdminPanel (SortedDictionaryManager sortedDictionaryManager){
        InitializeComponent();

        DataContext = sortedDictionaryManager;
        sortedDictionaryManager.RequestClose += () => this.Close();
    }
}