using EventListener.Models;

public class ActivityDetailViewModel
{
    public Activity Activity { get; set; }
    public List<UserJoinActivity> UserJoinActivity { get; set; }
    public int UserJoinActivityCount { get; set; }
    public bool isUserJoin { get; set; }
    public string ActivityId { get; set; }
    public string? UserImageUrl { get; set; }

    
}
