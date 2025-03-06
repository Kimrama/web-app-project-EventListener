using System.ComponentModel.DataAnnotations;
using EventListener.Models;

public class EditProfileViewModel
{
    public string UserName { get; set; }

    [Required]
    public string Firstname  { get; set; }

    [Required]
    public string Lastname { get; set; }

    [Required]
    public string Nickname { get; set; }

    [Required]
    public DateOnly Birthday { get; set; }

    [Required]
    public string Sex { get; set; }

    public string About { get; set; }
    public string UserImageUrl { get; set; }
    
    public List<ActivityTag> TagList { get; set; } = new List<ActivityTag>(); 
    public List<UserInterestActivityTag> UserInterestActivityTag { get; set; } = new List<UserInterestActivityTag>();
}