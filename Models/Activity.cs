using System.Diagnostics.CodeAnalysis;

namespace EventListener.Models;

public class Activity
{
    // pk = OwnerId + CreatedAt
    public required string OwnerId { get; set; }  //fk => pk of User
    public required string ActivityTagId { get; set; } //fk => pk of ActivityTag
    public required string ActivityName { get; set; }
    public required string Location { get; set; }
    public DateOnly Date { get; set; }
    public TimeSpan Time { get; set; }
    public string Detail { get; set; }
    public required int ParticipantLimit { get; set; }
    public string ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public required User User { get; set; }
    public required ActivityTag ActivityTag { get; set; }
    public ICollection<UserJoinActivity> UserJoinActivities { get; set; }
    public ICollection<ChatMessage> ChatMessages { get; set; } 
}
