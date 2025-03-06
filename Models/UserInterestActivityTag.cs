namespace EventListener.Models;

public class UserInterestActivityTag
{
    // pk = UserId + ActivityTagId
    public string UserId { get; set; }
    public string ActivityTagId { get; set; }

    public User User { get; set; }
    public ActivityTag ActivityTag { get; set; }
}
