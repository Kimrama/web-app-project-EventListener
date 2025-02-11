using EventListener.Models;

public class EditProfileViewModel
{
    public User User { get; set; } = new User(); 
    public List<ActivityTag> TagList { get; set; } = new List<ActivityTag>(); 
    public List<UserInterestActivityTag> UserInterestActivityTag { get; set; } = new List<UserInterestActivityTag>();
}