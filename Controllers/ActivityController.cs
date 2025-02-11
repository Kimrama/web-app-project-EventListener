using System.Globalization;
using EventListener.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventListener.Controllers;

public class ActivityController : Controller
{
    private readonly ApplicationDbContext _context;

    public ActivityController(ApplicationDbContext context)
    {
        _context = context;
    }

    public string EncodeBase64(string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes);
    }

    public string DecodeBase64(string input)
    {
        var bytes = Convert.FromBase64String(input);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    [HttpGet]
    [Route("Activity/Detail/{activityIdHash}")]
    public async Task<IActionResult> Detail(string activityIdHash)
    {
        var activityId = DecodeBase64(activityIdHash);

        var keys = activityId.Split(" ", 2);

        var activityCreatedAt = DateTime.ParseExact(keys[1], "yyyy-MM-dd HH:mm:ss.fffffff", CultureInfo.InvariantCulture);

        var activity = await _context.Activities
        .Include(a => a.User)
        .FirstOrDefaultAsync
        (
            a => a.OwnerId == keys[0] &&
            a.CreatedAt == activityCreatedAt
        );

        var usersJoinActivity = await _context.UserJoinActivities
        .Include(u => u.User)
        .Where(
            u => u.ActivityOwnerId == keys[0] &&
            u.ActivityCreatedAt == activityCreatedAt
        )
        .ToListAsync();

        var userJoinActivityCount = await _context.UserJoinActivities
        .Where(
            u => u.ActivityOwnerId == keys[0] &&
            u.ActivityCreatedAt == activityCreatedAt
        )
        .CountAsync();

        var model = new ActivityDetailViewModel
        {
            Activity = activity,
            UserJoinActivity = usersJoinActivity,
            UserJoinActivityCount = userJoinActivityCount
        };

        return View(model);
    }

    public IActionResult Create()
    {
        return View();
    }
}