namespace EventListener.Models;

public class Activity
{
    // pk = OwnerId + CreatedAt
    public string OwnerId { get; set; }  //fk => pk of User
    public string ActivityTagId { get; set; } //fk => pk of ActivityTag
    public string ActivityName { get; set; }
    public string Location { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan Time { get; set; }
    public string Detail { get; set; }
    public int ParticipantLimit { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
    public ActivityTag ActivityTag { get; set; }
    public ICollection<UserJoinActivity> UserJoinActivities { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; }
}
