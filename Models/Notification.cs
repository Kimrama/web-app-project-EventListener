namespace EventListener.Models;

public class Notification
{
    public int NotificationId { get; set; } //pk (surrogate key)
    public string UserId { get; set; } //fk => pk of User
    public string Message { get; set; }
    public DateTime ReceiveDate { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
}


