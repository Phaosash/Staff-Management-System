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
    private string? _selectedStaffName = "";
    private int? _selectedStaffId;

    public MasterFile MasterFile {
        get => _masterFile;
        set => SetProperty(ref _masterFile, value);
    }

    public ObservableCollection<StaffMember> StaffMembers {
        get => _staffMembers;
        set => SetProperty(ref _staffMembers, value);
    }

    public string? SearchTerm {
        get => _searchTerm;
        set => SetProperty(ref _searchTerm, value);
    }

    public string? SearchStaffName {
        get => _searchStaffName;
        set => SetProperty(ref _searchStaffName, value);
    }

    public int SearchStaffId {
        get => _searchStaffId;
        set => SetProperty(ref _searchStaffId, value);
    }

    public StaffMember SelectedStaffMember {
        get => _selectedStaffMember;
        set => SetProperty(ref _selectedStaffMember, value);
    }

    public bool ShouldFocusIdTextBox {
        get => _shouldFocusIdTextBox;
        set => SetProperty(ref _shouldFocusIdTextBox, value);
    }

    public bool ShouldFocusNameTextBox {
        get => _shouldFocusNameTextBox;
        set => SetProperty(ref _shouldFocusNameTextBox, value);
    }

    public string? SelectedStaffName {
        get => _selectedStaffName;
        set => SetProperty(ref _selectedStaffName, value);
    }

    public int? SelectedStaffId {
        get => _selectedStaffId;
        set => SetProperty(ref _selectedStaffId, value);
    }
}