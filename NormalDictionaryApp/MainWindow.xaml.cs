using Microsoft.Extensions.DependencyInjection;
using NormalDictionaryApp.Views;
using StaffManager.Classes;
using System.Windows;

namespace NormalDictionaryApp;

public partial class MainWindow : Window {
    public MainWindow (OrdinaryDictionaryManager ordinaryDictionaryManager){
        InitializeComponent();

        DataContext = ordinaryDictionaryManager;
        
        ordinaryDictionaryManager.RequestNewWindow += OnRequestNewWindow;
        ordinaryDictionaryManager.RequestApplicationClose += () => this.Close();
    }

    private void OnRequestNewWindow (){
        if (App.ServiceProvider != null){
            var adminPanel = App.ServiceProvider.GetRequiredService<AdminPanel>();
            adminPanel.Show();
        }
    }
}