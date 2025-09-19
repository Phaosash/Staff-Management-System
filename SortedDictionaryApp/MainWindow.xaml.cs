using Microsoft.Extensions.DependencyInjection;
using SortedDictionaryApp.Views;
using StaffManager.Classes;
using System.Windows;

namespace SortedDictionaryApp;

public partial class MainWindow : Window {
    private readonly SortedDictionaryManager _dContext;

    public MainWindow (){
        InitializeComponent();

        _dContext = new SortedDictionaryManager();

        DataContext = _dContext;

        _dContext.RequestNewWindow += OnRequestNewWindow;
        _dContext.RequestClose += () => this.Close();
    }

    private void OnRequestNewWindow (){
        if (App.ServiceProvider != null){
            var adminPanel = App.ServiceProvider.GetRequiredService<AdminPanel>();
            adminPanel.Show();
        }
    }
}