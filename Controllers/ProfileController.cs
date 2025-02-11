using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventListener.Data;
using EventListener.Models;
using EventListener.ViewModels.Profile;
using System.Linq;
using System.Threading.Tasks;

namespace EventListener.Controllers;
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
