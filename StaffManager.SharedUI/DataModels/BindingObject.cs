using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace StaffManager.SharedUI.DataModels;

public partial class BindingObject : ObservableObject {
    [ObservableProperty] private bool _shouldFocusObjectIdField;
    [ObservableProperty] private bool _shouldFocusObjectNameField;
    [ObservableProperty] private bool _shouldFocusButton;
    [ObservableProperty] private int? _selectedObjectId;
    [ObservableProperty] private string _searchTerm = string.Empty;
    [ObservableProperty] private string _selectedObjectName = string.Empty;
    [ObservableProperty] private string _newObjectName = string.Empty;
    [ObservableProperty] private string _updatedObjectName = string.Empty;
    [ObservableProperty] private string _generalFeedback = string.Empty;
    [ObservableProperty] private string _adminFeedback = string.Empty;
    [ObservableProperty] private SelectableObjects _selectedObject = new();
    [ObservableProperty] private ObservableCollection<SelectableObjects> _selectableObjects = [];
    [ObservableProperty] private IDictionary<int, string>? _readOnlyObject;
}