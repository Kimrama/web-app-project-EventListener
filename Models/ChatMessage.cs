using System;

namespace EventListener.Models;

public class ChatMessage
{
    public int ChatMessageId { get; set; } //pk (surrogate key)
    public string UserId { get; set; } //fk => pk of User
    public string ActivityOwnerId { get; set; } //fk => pk of Activity
    public DateTime ActivityCreatedAt { get; set; } //fk => pk of Activity
    public string Message { get; set; }
    public DateTime Date { get; set; }

    // Navigation Property

    public User User { get; set; }
    public Activity Activity { get; set; }
}
