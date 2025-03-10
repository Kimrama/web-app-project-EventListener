using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventListener.Data;
using EventListener.Models;
using EventListener.Services;
using System.Security.Claims;
using System.Globalization;
using System.Threading.Tasks;

namespace EventListener.Controllers;

public class NotificationController : Controller
{
    private readonly ApplicationDbContext _context; // เอาไว้ดึงฐานข้อมูลที่ต้องการ 
    private readonly UserManager<User> _userManager; //ใช้ดึงข้อมูลชื่อคนใช้งาน
    private readonly CloudinaryService _cloudinaryService;
    private User? _user;

    public NotificationController(ApplicationDbContext context, UserManager<User> userManager, CloudinaryService cloudinaryService)
    {
        _context = context;
        _userManager = userManager;
        _cloudinaryService = cloudinaryService;
    }
    public async Task<IActionResult> Notification()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);

        if (username != null)
        {
            _user = await _userManager.FindByNameAsync(username);
        }

        var activityJoins = await _context.UserJoinActivities
        .Where(a => a.UserId == username && a.Status == "Accept")
        .ToListAsync();


        foreach (var aj in activityJoins)
        {
            var activity = await _context.Activities
            .Where(a => a.OwnerId == aj.ActivityOwnerId && a.CreatedAt == aj.ActivityCreatedAt)
            .FirstOrDefaultAsync();

            var startDateTime = activity.StartDate.ToDateTime(TimeOnly.FromTimeSpan(activity.StartTime));
            var differentDateTime = startDateTime - DateTime.UtcNow.AddHours(7);

            Console.WriteLine($"now: {DateTime.UtcNow.AddHours(7)}");
            Console.WriteLine($"start: {startDateTime}");
            Console.WriteLine($"diff: {differentDateTime.TotalHours}");
            Console.WriteLine($"กิจกรรมจะเริ่มขึ้นตอน {activity.StartDate.ToString("dddd", new System.Globalization.CultureInfo("th-TH"))}, {activity.StartDate.AddYears(-543).ToString("d MMMM yyyy", new System.Globalization.CultureInfo("th-TH"))}, {activity.StartTime.ToString(@"hh\:mm")} น.");

            if (differentDateTime.TotalHours < 3 && !aj.IsNoti)
            {
                var activityIdHash = Base64Helper.EncodeBase64(activity.OwnerId + " " + activity.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US")));
                var notification = new Notification
                {
                    UserId = username,
                    ActivityIdString = activityIdHash,
                    Message = $"กิจกรรมจะเริ่มขึ้นตอน {activity.StartDate.ToString("dddd", new System.Globalization.CultureInfo("th-TH"))}, {activity.StartDate.AddYears(-543).ToString("d MMMM yyyy", new System.Globalization.CultureInfo("th-TH"))}, {activity.StartTime.ToString(@"hh\:mm")} น.",
                    ReceiveDate = startDateTime.AddHours(-3)
                };

                aj.IsNoti = true;

                _context.UserJoinActivities.Update(aj);
                await _context.SaveChangesAsync();

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
            }
        }

        var notifications = await _context.Notifications
        .Where(n => n.UserId == username)
        .ToListAsync();

        List<NotificationViewModel> models = [];
        foreach (var n in notifications)
        {
            var activityId = Base64Helper.DecodeBase64(n.ActivityIdString);
            var keys = activityId.Split(" ", 2);

            var activity = await _context.Activities
            .Where(a => a.OwnerId == keys[0] && a.CreatedAt.ToString() == keys[1])
            .FirstOrDefaultAsync();

            Console.WriteLine($"Activity Name: {activity.ActivityName}");

            models.Add(new NotificationViewModel
            {
                Id = n.NotificationId,
                ActivityIdEncode = n.ActivityIdString,
                ActivityName = activity.ActivityName,
                Message = n.Message,
                ReceiveDate = n.ReceiveDate,
            });
        }

        return View(models);
    }

    [HttpDelete]
    public async Task<JsonResult> DeleteNotification(int notificationId)
    {
        var counter = await _context.Notifications
            .Where(n => n.NotificationId == notificationId)
            .ExecuteDeleteAsync();

        if (counter > 0)
        {
            return Json(new { success = true, count = counter });
        }
        else
        {
            return Json(new { success = false, message = "ไม่พบการแจ้งเตือนนี้" });
        }
    }

}

