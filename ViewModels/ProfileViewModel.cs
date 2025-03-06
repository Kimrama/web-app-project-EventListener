using EventListener.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
public class ProfileViewModel
{
    public string Username { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string Nickname { get; set; }
    public string Birthday { get; set; }
    public int Age { get; set; }
    public string SexColor { get; set; }
    public string About { get; set; }
    public string UserImageUrl{ get; set; }
    public double Numpage { get; set; }
    public List<UserInterestActivityTag> InterestTags { get; set; } = new List<UserInterestActivityTag>();
    public List<ProfileFullJoinModel> JoinActivities { get; set; } = new List<ProfileFullJoinModel>();
    public List<string> ActivityCategory { get; set; } = new List<string>();
    public List<string> ActivityCategoryColor { get; set; } = new List<string>();

}
