using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    public string UserName { get; set; }

    [Required]
    public string Firstname { get; set; }

    [Required]
    public string Lastname { get; set; }

    [Required]
    public DateOnly Birthday { get; set; }

    [Required]
    public string Sex { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; }

    public List<string> Interests { get; set; }
}


