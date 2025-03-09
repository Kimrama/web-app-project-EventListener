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
        
        var notifications = await _context.Notifications
            .Where(
                u => u.UserId == username &&
                u.ReceiveDate <= DateTime.Now.AddHours(7)
            )
            .OrderByDescending(u => u.ReceiveDate)
            .ToListAsync();
        // var usersJoinActivity = await _context.UserJoinActivities
        // .Include(u => u.Activity)
        // .Where(
        //     u => u.UserId == username &&
        //     u.Status == "Accept"
        // )
        // .OrderByDescending(u => u.Activity.StartDate)
        // .ThenByDescending(u => u.Activity.StartTime)
        // .ToListAsync();
        
        List<NotificationViewModel> models = [];
        foreach (var noti in notifications)
        {
            string[] msg = noti.Message.Split(' ', 3);
            models.Add(new NotificationViewModel
            {   
                ReceiveDate = noti.ReceiveDate,
                ActivityIdEncode = msg[0],
                ActivityName = msg[1],
                Message = msg[2],
                Id = noti.NotificationId

            });
        }
        // foreach (var uja in usersJoinActivity)
        // {
        //     if(uja.Activity.StartDate.ToDateTime(TimeOnly.FromTimeSpan(uja.Activity.StartTime)) <= DateTime.Now.AddHours(1))
        //     {
        //         var activityName = uja.Activity.ActivityName;
        //         var startTime = uja.Activity.StartTime;
        //         var startDateTime = uja.Activity.StartDate.ToDateTime(TimeOnly.FromTimeSpan(startTime));
        //         var message = "กิจกรรมกำลังจะเริ่มวัน "+startDateTime.ToString(" dddd ที่ dd MMM yyyy, HH:mm น.");
        //         models.Add(new NotificationViewModel
        //         {
        //             ActivityName = activityName, 
        //             StartDateTime = startDateTime, 
        //             ActivityIdEncode = Base64Helper.EncodeBase64(uja.ActivityOwnerId + " " + uja.ActivityCreatedAt.ToString("yyyy-MM-dd HH:mm:ss", new CultureInfo("en-US"))),
        //             Message = message
        //         });
        //         var noti = new Notification{
        //             UserId = uja.UserId,
        //             Message = message
        //         };
        //         _context.Notifications.Add(noti);
        //     } 
        // }
        // await _context.SaveChangesAsync();

        return View(models);
    }  

    // [HttpDelete]
    // public async Task<JsonResult> DeleteNotification(int notificationId)
    // {
    //     var counter = await _context.Notifications.Where(n => n.NotificationId == notificationId).ExecuteDeleteAsync();
    //     return Json(new { success = true, count = counter });
    // }
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

