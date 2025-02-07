using System;
using Microsoft.EntityFrameworkCore;

namespace EventListener.Models;

public class ActivityTag
{
    public string Name { get; set; } //pk
    public string Category { get; set; }

    // Navigation Property
    public ICollection<Activity> Activities { get; set; }
    public ICollection<UserInterestActivityTag> UserInterestActivityTags { get; set; }
}

