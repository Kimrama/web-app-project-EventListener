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

    public async Task<IActionResult> Edit(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username is required");
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserName == username);

        if (user == null)
        {
            return NotFound($"User '{username}' not found.");
        }

        var tagList = await _context.ActivityTags.ToListAsync();

        var userInterestTag = await _context.UserInterestActivityTags
            .Where(u => u.UserId == username)
            .ToListAsync();

        var model = new EditProfileViewModel
        {
            User = user,
            UserInterestActivityTag = userInterestTag,
            TagList = tagList
        };

        return View(model);
    }
}
