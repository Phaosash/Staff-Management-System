using Microsoft.Extensions.DependencyInjection;
using SortedDictionaryApp.Views;
using StaffManager.Classes;
using System.Windows;

namespace SortedDictionaryApp;

public partial class MainWindow : Window {
    //  This constructor sets up the MainWindow by initializing its components and assigning its data context to an SortedDictionaryManager instance.
    //  It also handles two events from the manager: opening a new window and closing the application.
    public MainWindow (SortedDictionaryManager sortedDictionaryManager){
        InitializeComponent();

        DataContext = sortedDictionaryManager;

        sortedDictionaryManager.RequestNewWindow += OnRequestNewWindow;
        sortedDictionaryManager.RequestApplicationClose += () => this.Close();
    }

    //  This method handles requests to open a new window by retrieving an instance of AdminPanel
    //  from the application's service provider and displaying it, provided the service provider is
    //  available.
    private void OnRequestNewWindow (){
        if (App.ServiceProvider != null){
            var adminPanel = App.ServiceProvider.GetRequiredService<AdminPanel>();
            adminPanel.Show();
        }
    }
}