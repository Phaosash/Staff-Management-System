using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace StaffManager.DataModels;

public partial class StaffData: ObservableObject {
    private MasterFile _masterFile = new();
    private ObservableCollection<StaffMember> _staffMembers = [];
    private string? _searchTerm = string.Empty;
    private string? _searchStaffName = string.Empty;
    private int _searchStaffId;
    private StaffMember _selectedStaffMember = new();
    private bool _shouldFocusIdTextBox;
    private bool _shouldFocusNameTextBox;
    private string? _selectedStaffName = string.Empty;
    private int? _selectedStaffId;
    private string? _newStaffName = string.Empty;
    private string? _updatedStaffName = string.Empty;

    //  This property exposes a MasterFile instance with a getter
    //  and a setter that updates the backing field using SetProperty,
    //  which likely handles change notification.
    public MasterFile MasterFile {
        get => _masterFile;
        set => SetProperty(ref _masterFile, value);
    }

    //  This property manages a collection of StaffMember objects using an ObservableCollection,
    //  with a getter and setter that updates the backing field through SetProperty to support
    //  change notification.
    public ObservableCollection<StaffMember> StaffMembers {
        get => _staffMembers;
        set => SetProperty(ref _staffMembers, value);
    }

    //  This nullable string propertyuses a getter and setter that updates the backing field
    //  via SetProperty, enabling change notifications.
    public string? SearchTerm {
        get => _searchTerm;
        set => SetProperty(ref _searchTerm, value);
    }

    //  This nullable string property provides access to a backing field with a
    //  getter and setter that uses SetProperty to handle updates and notify changes.
    public string? SearchStaffName {
        get => _searchStaffName;
        set => SetProperty(ref _searchStaffName, value);
    }

    //  This integer property uses a getter and setter to access a backing field, with
    //  SetProperty ensuring updates trigger change notifications.
    public int SearchStaffId {
        get => _searchStaffId;
        set => SetProperty(ref _searchStaffId, value);
    }

    //  This property manages the currently selected StaffMember with a getter and setter
    //  that update the backing field using SetProperty to support change notification.
    public StaffMember SelectedStaffMember {
        get => _selectedStaffMember;
        set => SetProperty(ref _selectedStaffMember, value);
    }

    //  This boolean property uses a getter and setter with SetProperty to update its
    //  backing field and notify when the value changes, likely controlling focus behavior
    //  in the UI.
    public bool ShouldFocusIdTextBox {
        get => _shouldFocusIdTextBox;
        set => SetProperty(ref _shouldFocusIdTextBox, value);
    }

    //  This boolean property uses a getter and setter with SetProperty to update its
    //  backing field and trigger change notifications, typically to control focus on
    //  the name input field in the UI.
    public bool ShouldFocusNameTextBox {
        get => _shouldFocusNameTextBox;
        set => SetProperty(ref _shouldFocusNameTextBox, value);
    }

    //  This nullable string property provides access to its backing field through a
    //  getter and setter that use SetProperty to handle updates and notify changes.
    public string? SelectedStaffName {
        get => _selectedStaffName;
        set => SetProperty(ref _selectedStaffName, value);
    }

    //  This nullable integer property manages its backing field with a getter and
    //  setter that use SetProperty to update the value and notify any changes.
    public int? SelectedStaffId {
        get => _selectedStaffId;
        set => SetProperty(ref _selectedStaffId, value);
    }

    //  This nullable string property uses a getter and setter with SetProperty
    //  to update its backing field and trigger change notifications, likely for
    //  adding new staff members.
    public string? NewStaffName {
        get => _newStaffName;
        set => SetProperty(ref _newStaffName, value);
    }

    //  This nullable string property manages its backing field through a getter and
    //  setter that use SetProperty to handle updates and notify changes, typically
    //  for editing existing staff names.
    public string? UpdatedStaffName {
        get => _updatedStaffName;
        set => SetProperty(ref _updatedStaffName, value);
    }
}