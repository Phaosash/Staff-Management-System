using Microsoft.Extensions.DependencyInjection;
using SortedDictionaryApp.Views;
using StaffManager.Classes;
using System.Windows;

namespace SortedDictionaryApp;

public partial class MainWindow : Window {
    public MainWindow (SortedDictionaryManager sortedDictionaryManager){
        InitializeComponent();

        DataContext = sortedDictionaryManager;

        sortedDictionaryManager.RequestNewWindow += OnRequestNewWindow;
        sortedDictionaryManager.RequestApplicationClose += () => this.Close();
    }

    private void OnRequestNewWindow (){
        if (App.ServiceProvider != null){
            var adminPanel = App.ServiceProvider.GetRequiredService<AdminPanel>();
            adminPanel.Show();
        }
    }
}