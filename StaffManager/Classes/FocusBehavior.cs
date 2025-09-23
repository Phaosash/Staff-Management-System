using System.Windows;
using System.Windows.Controls;

namespace StaffManager.Classes;

public static class FocusBehavior {
    //  This code defines an attached dependency property named IsFocused of type bool on the FocusBehavior class,
    //  enabling two-way data binding and specifying a callback method to handle changes to its value.
    public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusBehavior),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsFocusedChanged));

    //  This method retrieves the current value of the IsFocused attached property from a given DependencyObject.
    public static bool GetIsFocused(DependencyObject obj) => (bool)obj.GetValue(IsFocusedProperty);

    //  This method sets the value of the IsFocused attached property on a specified DependencyObject.
    public static void SetIsFocused(DependencyObject obj, bool value) => obj.SetValue(IsFocusedProperty, value);

    //  This method handles changes to the IsFocused property by setting focus to the TextBox, selecting all its
    //  text when the property becomes true, and then resetting the property to false.
    private static void OnIsFocusedChanged (DependencyObject d, DependencyPropertyChangedEventArgs e){
        if (d is TextBox textBox && e.NewValue is bool isFocused && isFocused){
            textBox.Focus();
            textBox.SelectAll();
            SetIsFocused(textBox, false);
        }
    }
}