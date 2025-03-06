using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventListener.Data;
using EventListener.Models;
using EventListener.Services;
using System.Security.Claims;
using System.Globalization;

namespace EventListener.Controllers;

public class NotificationController : Controller
{
    private readonly ApplicationDbContext _context; // เอาไว้ดึงฐานข้อมูลที่ต้องการ 
    private readonly UserManager<User> _userManager; //ใช้ดึงข้อมูลชื่อคนใช้งาน
    private readonly CloudinaryService _cloudinaryService;
 

    public NotificationController(ApplicationDbContext context, UserManager<User> userManager, CloudinaryService cloudinaryService)
    {
        _context = context;
        _userManager = userManager;
        _cloudinaryService = cloudinaryService;
    }
     public async Task<IActionResult> Notification()
    {   var username = User.FindFirstValue(ClaimTypes.Name); //inspect login user

        if (string.IsNullOrEmpty(username))
        {
            return Forbid();
        }

        var user = await _userManager.FindByNameAsync(username);//fetch db ready to use

        if (user == null)
        {
            return NotFound();
        }
        
        var usersJoinActivity = await _context.UserJoinActivities
        .Include(u => u.Activity)
        .Where(
            u => u.UserId == username &&
            u.Status == "Accept"
        )
        .OrderByDescending(u => u.Activity.StartDate)
        .ThenByDescending(u => u.Activity.StartTime)
        .ToListAsync();

        List<NotificationViewModel> models = [];
        foreach (var uja in usersJoinActivity)
        {
            if(uja.Activity.StartDate.ToDateTime(TimeOnly.FromTimeSpan(uja.Activity.StartTime)) <= DateTime.Now.AddHours(1))
            {
                var activityName = uja.Activity.ActivityName;
                var startTime = uja.Activity.StartTime;
                var startDateTime = uja.Activity.StartDate.ToDateTime(TimeOnly.FromTimeSpan(startTime));
                models.Add(new NotificationViewModel
                {
                    ActivityName = activityName, 
                    StartDateTime = startDateTime, 
                    ActivityIdEncode = Base64Helper.EncodeBase64(uja.ActivityOwnerId + " " + uja.ActivityCreatedAt.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"))),
                });
            }
        }

        return View(models);
    }
   
}

