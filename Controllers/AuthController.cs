using System.Globalization;
using EventListener.Data;
using EventListener.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _context;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public IActionResult Register() => View();

    [HttpPost("auth/register")]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new User
        {
            UserName = model.UserName,
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Birthday = model.Birthday,
            Sex = model.Sex,
            About = " ",
            UserImageUrl = " "
        };

        var userInterestActivityTag = new List<UserInterestActivityTag>();

        foreach (var i in model.Interests)
        {
            userInterestActivityTag.Add(new UserInterestActivityTag
            {
                UserId = model.UserName,
                ActivityTagId = i
            });
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            _context.UserInterestActivityTags.AddRange(userInterestActivityTag);
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Login", "Auth");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }
        }

        return View();
    }

    public IActionResult Login() => View();
}