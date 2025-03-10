namespace EventListener.Models;

public class UserJoinActivity
{
    //pk = UserId + ActivityOwnerId + ActivityCreatedAt
    public string UserId { get; set; } //fk => pk of User that join activity
    public string ActivityOwnerId { get; set; } //fk => key of Activity
    public DateTime ActivityCreatedAt { get; set; } //fk => key of Activity
    public DateTime? RequestJoinDate { get; set; } = DateTime.UtcNow;
    public DateTime? JoinDate { get; set; }
    public string? Status { get; set; }
    public bool IsNoti {get; set;} = false;

    public User User { get; set; }
    public Activity Activity { get; set; }
}
