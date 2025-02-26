using EventListener.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
public class ProfileFullJoinModel
{
    public string UserId { get; set; } //fk => pk of User that join activity
    public string ActivityOwnerId { get; set; } //fk => key of Activity
    public string? Status { get; set; }
    public string OwnerId { get; set; }  //fk => pk of User
    public string ActivityTagId { get; set; } //fk => pk of ActivityTag
    public string ActivityName { get; set; }
    public string Location { get; set; }
    public DateOnly StartDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
