namespace EventListener.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dotnetcoreidentity.Data;
using EventListener.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
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
            .Where(u => u.UserId == user.Id)
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
