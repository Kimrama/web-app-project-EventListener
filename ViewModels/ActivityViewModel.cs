namespace EventListener.Models;

public class ActivityViewModel
{
    public string ActivityIdEncode { get; set; }
    public string ActivityTagId { get; set; }
    public string ActivityName { get; set; }
    public string Location { get; set; }
    public DateOnly StartDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public int ParticipantLimit { get; set; }
    public string ActivityImageUrl { get; set; }
    public string ActivityTagCategory { get; set; }
    public int UserJoinActivityCount { get; set; }
}