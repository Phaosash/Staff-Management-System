using StaffManager.Classes;
using System.Windows;

namespace SortedDictionaryApp.Views;

public partial class AdminPanel : Window {
    public AdminPanel (SortedDictionaryManager sortedDictionaryManager){
        InitializeComponent();

        DataContext = sortedDictionaryManager;
        sortedDictionaryManager.RequestClose += () => this.Close();
    }
}