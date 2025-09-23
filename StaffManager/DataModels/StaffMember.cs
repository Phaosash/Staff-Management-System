using CommunityToolkit.Mvvm.ComponentModel;

namespace StaffManager.DataModels;

public class StaffMember: ObservableObject {
    private int? id;
    private string? name;

    //  This property exposes a nullable integer Id with a getter and a setter that
    //  updates the backing field using a method (`SetProperty`) that likely handles
    //  change notification.
    public int? Id {
        get => id;
        set => SetProperty(ref id, value);
    }

    //  This nullable string property Name uses a getter and a setter that updates the
    //  backing field through SetProperty, likely triggering change notifications.
    public string? Name { 
        get => name; 
        set => SetProperty(ref name, value);
    }
}