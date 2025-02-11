using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EventListener.Data;
using EventListener.Models;

public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ApplicationDbContext _context;

    public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new User
        {
            UserName = model.UserName,
            Firstname = model.Firstname,
            Lastname = model.Lastname,
            Nickname = model.Nickname,
            Birthday = model.Birthday,
            Sex = model.Sex,
            Email = model.UserName + "@gmail.com"
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
            await _signInManager.SignInAsync(user, isPersistent: false);

            try
            {
                _context.UserInterestActivityTags.AddRange(userInterestActivityTag);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);

                return View(model);
            }

            return RedirectToAction("Login", "Account");
        }

        foreach (var error in result.Errors)
        {
            Console.WriteLine(error.Description);
        }

        return View(model);
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
        return RedirectToAction("Login", "Account");
    }
}


