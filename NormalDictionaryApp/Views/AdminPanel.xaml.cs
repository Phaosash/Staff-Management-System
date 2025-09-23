using StaffManager.Classes;
using System.Windows;

namespace NormalDictionaryApp.Views;

public partial class AdminPanel : Window {
    //  This constructor initializes the user interface and sets its data context to an instance of OrdinaryDictionaryManager.
    //  It also subscribes to the RequestClose event of the manager, closing the panel when that event is triggered.
    public AdminPanel (OrdinaryDictionaryManager ordinaryDictionaryManager){
        InitializeComponent();

        DataContext = ordinaryDictionaryManager;
        ordinaryDictionaryManager.RequestClose += () => this.Close();
    }
}