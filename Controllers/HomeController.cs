using EventListener.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EventListener.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using EventListener.Services;

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
    [HttpGet]
    public async Task<IActionResult> Index()
    {

        
        var activities = await _context.Activities
        .Include(a => a.ActivityTag)
        .Include(b => b.UserJoinActivities.Where(uja => uja.Status == "Accept"))
        .ToListAsync();
        
        var activityViewModels = activities.OrderByDescending(a => a.CreatedAt)
        .Select(c => new ActivityViewModel
        {
            ActivityIdEncode = Base64Helper.EncodeBase64(c.OwnerId + " " + c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"))),
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

[HttpGet]
public async Task<JsonResult> FetchActivityData()
{
    var activities = await _context.Activities
        .Include(a => a.ActivityTag)
        .Include(a => a.UserJoinActivities)
            .ThenInclude(uja => uja.User)
        .OrderByDescending(a => a.CreatedAt)
        .Select(c => new ActivityViewModel
        {
            ActivityIdEncode = Base64Helper.EncodeBase64(c.OwnerId + " " + c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"))),
            ActivityTagId = c.ActivityTagId,
            ActivityName = c.ActivityName,
            Location = c.Location,
            StartDate = c.StartDate,
            StartTime = c.StartTime,
            ParticipantLimit = c.ParticipantLimit,
            ActivityImageUrl = c.ActivityImageUrl,
            ActivityTagCategory = c.ActivityTag.Category,
            UserJoinActivityCount = c.UserJoinActivities.Count(uja => uja.Status == "Accept"), 
            UserJoinActivity = c.UserJoinActivities
                .Where(uja => uja.Status == "Accept")
                .Select(uja => new UserJoinActivity
                {
                    Status = uja.Status,
                    User = new User
                    {
                        UserImageUrl = uja.User.UserImageUrl
                    }
                }).ToList()
        })
        .ToListAsync();

    return Json(activities);
}

[HttpGet]
public async Task<JsonResult> FilterActivityData(DateTime? startDate, DateTime? endDate, string[]? tags, string? searchWord)
{

    var query = _context.Activities
        .Include(a => a.ActivityTag)
        .Include(a => a.UserJoinActivities)
            .ThenInclude(uja => uja.User)
        .AsQueryable();

    if (startDate.HasValue)
    {
        var startDateOnly = DateOnly.FromDateTime(startDate.Value);
        query = query.Where(a => a.StartDate >= startDateOnly);
    }
    if (endDate.HasValue)
    {
        var endDateOnly = DateOnly.FromDateTime(endDate.Value);
        query = query.Where(a => a.StartDate <= endDateOnly);
    }

    if (tags != null && tags.Any())
    {
        query = query.Where(a => tags.Contains(a.ActivityTagId)); 
    }

    if (!string.IsNullOrEmpty(searchWord))
    {
        query = query.Where(a => a.ActivityName.Contains(searchWord));
    }
    
    var activities = await query.OrderByDescending(a => a.CreatedAt)
        .Select(c => new ActivityViewModel
        {
            ActivityIdEncode = Base64Helper.EncodeBase64(c.OwnerId + " " + c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)),
            ActivityTagId = c.ActivityTagId,
            ActivityName = c.ActivityName,
            Location = c.Location,
            StartDate = c.StartDate,
            StartTime = c.StartTime,
            ParticipantLimit = c.ParticipantLimit,
            ActivityImageUrl = c.ActivityImageUrl,
            ActivityTagCategory = c.ActivityTag.Category,
            UserJoinActivityCount = c.UserJoinActivities.Count(uja => uja.Status == "Accept"),
            UserJoinActivity = c.UserJoinActivities
                .Where(uja => uja.Status == "Accept")
                .Select(uja => new UserJoinActivity
                {
                    Status = uja.Status,
                    User = new User { UserImageUrl = uja.User.UserImageUrl }
                }).ToList()
        })
        .ToListAsync();

    return Json(activities);
}


}
