using System.Windows;
using System.Windows.Controls;

namespace StaffManager.Classes;

//  This class is used for setting and clearing the focus of the different list boxes in the different UI's
public static class FocusBehavior {
    public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusBehavior),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsFocusedChanged));

    public static bool GetIsFocused(DependencyObject obj) => (bool)obj.GetValue(IsFocusedProperty);

    public static void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);

    private static void OnIsFocusedChanged (DependencyObject d, DependencyPropertyChangedEventArgs e){
        if (d is TextBox textBox && e.NewValue is bool isFocused && isFocused){
            textBox.Focus();
            textBox.SelectAll();
            SetIsFocused(textBox, false);
        }
    }
}