using StaffManager.SharedUI.Classes;
using System.Windows;

namespace StaffManager.SharedUI.Views;

public partial class AdminUI : Window {
    public AdminUI (UIInstanceManager uIInstance){
        InitializeComponent();
        DataContext = uIInstance;
        uIInstance.RequestWindowClose += () => this.Close();
    }
}