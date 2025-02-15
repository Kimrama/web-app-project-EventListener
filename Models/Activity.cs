namespace EventListener.Models;

public class Activity
{
    // pk = OwnerId + CreatedAt
    public string OwnerId { get; set; }  //fk => pk of User
    public string ActivityTagId { get; set; } //fk => pk of ActivityTag
    public string ActivityName { get; set; }
    public string Location { get; set; }
    public DateOnly StartDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public string Detail { get; set; }
    public int ParticipantLimit { get; set; }
    public string ActivityImageUrl { get; set; }
    public bool? IsApproveBeforeJoinActivity { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
    public ActivityTag ActivityTag { get; set; }
    public ICollection<UserJoinActivity> UserJoinActivities { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; }
}
