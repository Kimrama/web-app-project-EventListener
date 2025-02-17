using EventListener.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EventListener.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EventListener.Controllers;
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;


    public HomeController(ILogger<HomeController> logger, UserManager<User> userManager, ApplicationDbContext context)
    {
        _logger = logger;
        _userManager = userManager;
        _context = context;
    }
    public string EncodeBase64(string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes);
    }
    [HttpGet]
    public async Task<IActionResult> Index()
    {

        
        var activities = await _context.Activities
        .Include(a => a.ActivityTag)
        .Include(b => b.UserJoinActivities)
        .ToListAsync();
        
        var activityViewModels = activities.Select(c => new ActivityViewModel
        {
            ActivityIdEncode = EncodeBase64(c.OwnerId + " " + c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"))),
            ActivityTagId = c.ActivityTagId,
            ActivityName = c.ActivityName,
            Location = c.Location,
            StartDate = c.StartDate,
            StartTime = c.StartTime,
            ParticipantLimit = c.ParticipantLimit,
            ActivityImageUrl = c.ActivityImageUrl,
            ActivityTagCategory = c.ActivityTag.Category,
            UserJoinActivityCount = c.UserJoinActivities.Count()
        }).ToList();

        if (User.Identity.IsAuthenticated) {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == User.Identity.Name);
            
            if (currentUser != null) {
                ViewData["avatarUrl"] = currentUser.UserImageUrl;
            } else {
                return NotFound("User not found");
            }
        } else {

            ViewData["avatarUrl"] = "NULL";
        }
 
        return View(activityViewModels);
    }
}