using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EventListener.Data;
using EventListener.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EventListener.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ProfileController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(username))
        {
            return Forbid();
        }

        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        var model = new ProfileViewModel
        {
            UserName = user.UserName,
            Firstname = user.Firstname,
            Lastname = user.Lastname
        };

        return View(model);
    }

    public IActionResult Me()
    {
        return View();
    }

    public IActionResult Person()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username is required");
        }

        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return NotFound($"User '{username}' not found.");
        }

        var tagList = await _context.ActivityTags.ToListAsync();

        var userInterestTag = await _context.UserInterestActivityTags
            .Where(u => u.UserId == username)
            .Include(u => u.ActivityTag)
            .ToListAsync();

        var model = new EditProfileViewModel
        {
            User = user,
            UserInterestActivityTag = userInterestTag,
            TagList = tagList
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {

        // if (!ModelState.IsValid)
        // {
        //     foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        //     {
        //         Console.WriteLine($"Validation Error: {error.ErrorMessage}");
        //     }
        //     return View(model);
        // }
        string username = HttpContext.User.Identity?.Name;
        var user = await _context.Users.FindAsync(username);
        if (user == null) return NotFound();

        user.Firstname = model.User.Firstname;
        user.Lastname = model.User.Lastname;
        user.Birthday = model.User.Birthday;
        user.Sex = model.User.Sex;
        user.About = model.User.About;


        _context.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Me"); 
    }
}
