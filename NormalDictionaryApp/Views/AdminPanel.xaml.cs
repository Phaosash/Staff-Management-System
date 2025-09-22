using StaffManager.Classes;
using System.Windows;

namespace NormalDictionaryApp.Views;

public partial class AdminPanel : Window {
    public AdminPanel (OrdinaryDictionaryManager ordinaryDictionaryManager){
        InitializeComponent();

        DataContext = ordinaryDictionaryManager;
        ordinaryDictionaryManager.RequestClose += () => this.Close();
    }
}