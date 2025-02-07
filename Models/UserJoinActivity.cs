using System;
using Microsoft.EntityFrameworkCore;

namespace EventListener.Models;

public class UserJoinActivity
{
    public string UserId { get; set; } //fk => pk of User
    public string ActivityOwnerId { get; set; } //fk => pk of Activity
    public DateTime ActivityCreatedAt { get; set; } //fk => pk of Activity
    public DateTime JoinDate { get; set; } 

    // Navigation Property
    public User User { get; set; }
    public Activity Activity { get; set; }
}
