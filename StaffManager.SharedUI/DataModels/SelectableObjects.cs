using CommunityToolkit.Mvvm.ComponentModel;

namespace StaffManager.SharedUI.DataModels;

internal partial class SelectableObjects: ObservableObject {
    [ObservableProperty] private int? _id;
    [ObservableProperty] private string _name = string.Empty;
}