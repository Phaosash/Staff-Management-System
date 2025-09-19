using CommunityToolkit.Mvvm.ComponentModel;

namespace StaffManager.DataModels;

public class StaffMember: ObservableObject {
    private int? id;
    private string? name;


    public int? Id {
        get => id;
        set => SetProperty(ref id, value);
    }

    public string? Name { 
        get => name; 
        set => SetProperty(ref name, value); 
    }
}