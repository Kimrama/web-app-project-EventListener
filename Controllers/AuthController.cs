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
        Console.WriteLine("test");

        if (!ModelState.IsValid) return View(model);

        var user = new User
        {
            UserName = model.UserName,
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Nickname = model.Nickname,
            Birthday = model.Birthday,
            Sex = model.Sex,
        };

        var userInterestActivityTag = new List<UserInterestActivityTag>();

        foreach (var i in model.Interests)
        {
            userInterestActivityTag.Add(new UserInterestActivityTag
            {
                UserId = model.UserName,
                ActivityTagId = i
            });
        }
        ;

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

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        ModelState.AddModelError("", "Invalid login attempt.");

        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}


