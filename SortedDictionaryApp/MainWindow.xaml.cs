using StaffManager.Classes;
using System.Windows;

namespace SortedDictionaryApp;

public partial class MainWindow : Window {
    private readonly SortedDictionaryManager _dContext;

    public MainWindow (){
        InitializeComponent();

        _dContext = new SortedDictionaryManager();

        DataContext = _dContext;
    }
}