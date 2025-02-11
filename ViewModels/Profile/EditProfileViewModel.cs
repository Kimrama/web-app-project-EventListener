using EventListener.Models;
namespace EventListener.ViewModels.Profile;

public class EditProfileViewModel
{
    public User User { get; set; } = new User(); // ✅ ป้องกัน Null
    public List<ActivityTag> TagList { get; set; } = new List<ActivityTag>(); // ✅ ป้องกัน Null
    public List<UserInterestActivityTag> UserInterestActivityTag { get; set; } = new List<UserInterestActivityTag>(); // ✅ ป้องกัน Null
}