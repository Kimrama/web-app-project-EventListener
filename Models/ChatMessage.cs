namespace EventListener.Models;

public class ChatMessage
{
    public int ChatMessageId { get; set; } //pk (surrogate key)
    public string SenderId { get; set; } //fk => pk of User
    public string ActivityOwnerId { get; set; } //fk => key of Activity
    public DateTime ActivityCreatedAt { get; set; } //fk => key of Activity
    public string Message { get; set; }
    public DateTime SendDate { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
    public Activity Activity { get; set; }
}
