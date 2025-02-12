using Microsoft.AspNetCore.Identity;

namespace EventListener.Models;

public class User : IdentityUser
{
    //pk = UserName
    public string? Firstname  { get; set; }
    public string? Lastname { get; set; }
    public string? Nickname { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? Sex { get; set; }
    public string? About { get; set; }
    public string? UserImageUrl { get; set; }

    public ICollection<Activity> Activities { get; set; }
    public ICollection<UserInterestActivityTag> UserInterestActivityTags { get; set; }
    public ICollection<UserJoinActivity> UserJoinActivities { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; }
}
