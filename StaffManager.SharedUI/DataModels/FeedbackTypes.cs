namespace StaffManager.SharedUI.DataModels;

internal class FeedbackTypes {
    public enum FeedbackChoices { 
        Warning,
        Information,
        Error,
        None
    }

    public FeedbackChoices CurrentFeedbackType { get; set; }

    public FeedbackTypes (){
        CurrentFeedbackType = FeedbackChoices.None;        
    }
}