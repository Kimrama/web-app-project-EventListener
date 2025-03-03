using System.Globalization;
using System.Security.Claims;
using EventListener.Data;
using EventListener.Models;
using EventListener.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace EventListener.Controllers;

public class ActivityController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly CloudinaryService _cloudinaryService;
    private User? _user;

    public ActivityController(ApplicationDbContext context, UserManager<User> userManager, CloudinaryService cloudinaryService)
    {
        _context = context;
        _userManager = userManager;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet]
    [Route("Activity/Detail/{activityIdHash}")]
    public async Task<IActionResult> Detail(string activityIdHash)
    {
        var activityId = Base64Helper.DecodeBase64(activityIdHash);

        var keys = activityId.Split(" ", 2);

        var username = User.FindFirstValue(ClaimTypes.Name);

        if (username != null)
        {
            _user = await _userManager.FindByNameAsync(username);
        }

        var activity = await _context.Activities
        .Include(a => a.User)
        .FirstOrDefaultAsync
        (
            a => a.OwnerId == keys[0] &&
            a.CreatedAt.ToString() == keys[1]
        );

        var usersJoinActivity = await _context.UserJoinActivities
        .Include(u => u.User)
        .Where(
            u => u.ActivityOwnerId == keys[0] &&
            u.ActivityCreatedAt.ToString() == keys[1]
        )
        .ToListAsync();

        var userJoinActivityCount = await _context.UserJoinActivities
        .Where(
            u => u.ActivityOwnerId == keys[0] &&
            u.ActivityCreatedAt.ToString() == keys[1]
        )
        .CountAsync();

        var isUserJoin = await _context.UserJoinActivities.AnyAsync(
            uja => uja.UserId == username &&
            uja.ActivityOwnerId == keys[0] &&
            uja.ActivityCreatedAt.ToString() == keys[1]
        );

        var model = new ActivityDetailViewModel
        {
            Activity = activity,
            UserJoinActivity = usersJoinActivity,
            UserJoinActivityCount = userJoinActivityCount,
            isUserJoin = isUserJoin,
            ActivityId = activityIdHash,
            UserImageUrl = _user?.UserImageUrl
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> JoinActivity([FromBody] JoinActivityDto dto)
    {
        try
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            var activityId = Base64Helper.DecodeBase64(dto.ActivityIdHash);
            var keys = activityId.Split(" ", 2);

            var activity = await _context.Activities
            .Where(
                a => a.OwnerId == keys[0] &&
                a.CreatedAt.ToString() == keys[1]
            )
            .FirstOrDefaultAsync();

            var usersJoinActivity = await _context.UserJoinActivities
            .Include(u => u.User)
            .Where(
                u => u.ActivityOwnerId == keys[0] &&
                u.ActivityCreatedAt.ToString() == keys[1] &&
                u.Status == "Accept"
            )
            .ToListAsync();

            var userJoinActivityCount = usersJoinActivity.Count;

            if (userJoinActivityCount == activity.ParticipantLimit)
            {
                Console.WriteLine("test");
                return BadRequest(new
                {
                    message = "ไม่สามารถเข้าร่วมกิจกรรมได้ เนื่องจากสมาชิกครบเเล้ว"
                });
            }

            var userJoinActivity = new UserJoinActivity
            {
                UserId = username,
                ActivityOwnerId = keys[0],
                ActivityCreatedAt = DateTime.ParseExact(keys[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                Status = "wait"
            };
            var existingJoinActivity = await _context.UserJoinActivities
            .FirstOrDefaultAsync(u =>
                u.UserId == userJoinActivity.UserId &&
                u.ActivityOwnerId == userJoinActivity.ActivityOwnerId &&
                u.ActivityCreatedAt == userJoinActivity.ActivityCreatedAt);
            if (existingJoinActivity != null)
            {  
                if (existingJoinActivity.Status == "exit") { existingJoinActivity.Status = "wait";}
                else if (existingJoinActivity.Status == "exit2") { existingJoinActivity.Status = "wait2"; }
                else if (existingJoinActivity.Status == "exit3") { existingJoinActivity.Status = "wait3"; }
                else if (existingJoinActivity.Status == "Deny") { existingJoinActivity.Status = "wait2"; }
                else if (existingJoinActivity.Status == "Deny2") { existingJoinActivity.Status = "wait3"; }
                _context.UserJoinActivities.Update(existingJoinActivity);
            }
            else
            {
                _context.UserJoinActivities.Add(userJoinActivity);
            }

            await _context.SaveChangesAsync();

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500, new { message = "Internal Server Error", error = ex.Message });
        }
    }

    [HttpPost]
    public JsonResult UpdateJoinStatus(string ownerId, DateTime createDate, string joinUser, string status)
    {
        try
        {
            var userActivity = _context.UserJoinActivities
                .FirstOrDefault(a => a.ActivityOwnerId == ownerId &&
                                    a.ActivityCreatedAt == createDate &&
                                    a.UserId == joinUser);

            if (userActivity != null)
            {
                if (status == "Accept")
                {
                    if (userActivity.Status == "wait" || userActivity.Status == "wait2" || userActivity.Status == "wait3") { userActivity.Status = "Accept"; }
                }
                else if (status == "Deny")
                {
                    if (userActivity.Status == "wait") { userActivity.Status = "Deny"; }
                    else if (userActivity.Status == "wait2") { userActivity.Status = "Deny2"; }
                    if (userActivity.Status == "wait3") { userActivity.Status = "Deny3"; }
                }
                else if (status == "exit")
                {
                    if (userActivity.Status == "wait") { userActivity.Status = "exit"; }
                    else if (userActivity.Status == "wait2") { userActivity.Status = "exit2"; }
                    else if (userActivity.Status == "wait3") { userActivity.Status = "exit3"; }
                    else if (userActivity.Status == "Accept") {userActivity.Status = "exit";}
                }
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "User not found" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error: " + ex.Message });
        }
    }


    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var activityTags = await _context.ActivityTags.ToListAsync();

        ViewBag.activityTags = activityTags;

        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateActivityViewModel model, IFormFile file)
    {

        if (!ModelState.IsValid) return View(model);

        if (file == null || file.Length == 0)
        {
            ModelState.AddModelError("", "กรุณาเลือกไฟล์รูปภาพ");
            Console.WriteLine("ยังไม่ได้อัพโหลดรูปภาพ");
            return View();
        }
        else
        {
            Console.WriteLine("อัพโหลดรูปภาพเเล้ว");
        }

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

        DateOnly startDate = DateOnly.FromDateTime(model.StartDateTime);
        TimeSpan startTime = model.StartDateTime.TimeOfDay;

        var uploadResult = await _cloudinaryService.UploadImageAsync(file);
        if (uploadResult == null)
        {
            ModelState.AddModelError("", "อัปโหลดไม่สำเร็จ");
            return View();
        }

        var activity = new Activity
        {
            OwnerId = user.UserName,
            ActivityTagId = model.ActivityTag,
            ActivityName = model.ActivityName,
            Location = model.Location,
            StartDate = startDate,
            StartTime = startTime,
            Detail = model.Detail,
            ParticipantLimit = model.ParticipantLimit,
            ActivityImageUrl = uploadResult.SecureUrl.AbsoluteUri,
        };

        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet]
    [Route("Activity/Edit/{activityIdHash}")]
    public async Task<IActionResult> Edit(string activityIdHash)
    {

        var activityId = Base64Helper.DecodeBase64(activityIdHash);

        var keys = activityId.Split(" ", 2);

        var username = User.FindFirstValue(ClaimTypes.Name);

        if (username != keys[0])
        {
            return Forbid();
        }

        var activityTags = await _context.ActivityTags.ToListAsync();
        ViewBag.activityTags = activityTags;

        var activity = await _context.Activities
        .Where(a => a.OwnerId == keys[0] && a.CreatedAt.ToString() == keys[1])
        .Select(a => new { a.ActivityTagId, a.ActivityName, a.Location, a.StartDate, a.StartTime, a.ParticipantLimit, a.Detail, a.ActivityImageUrl })
        .FirstOrDefaultAsync();

        var model = new EditActivityViewModel
        {
            ActivityId = activityIdHash,
            ActivityTag = activity.ActivityTagId,
            ActivityName = activity.ActivityName,
            Location = activity.Location,
            StartDateTime = activity.StartDate.ToDateTime(TimeOnly.FromTimeSpan(activity.StartTime)),
            ParticipantLimit = activity.ParticipantLimit,
            Detail = activity.Detail,
        };

        ViewBag.ActivityImageUrl = activity.ActivityImageUrl;

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [Route("Activity/Edit/{activityIdHash}")]
    public async Task<IActionResult> Edit(string activityIdHash, EditActivityViewModel model, IFormFile file)
    {
        var activityId = Base64Helper.DecodeBase64(activityIdHash);

        var keys = activityId.Split(" ", 2);

        var activity = await _context.Activities
        .Where(a => a.OwnerId == keys[0] && a.CreatedAt.ToString() == keys[1])
        .FirstOrDefaultAsync();

        var oldDateTime = activity.StartDate.ToDateTime(TimeOnly.FromTimeSpan(activity.StartTime));
        var newDateTime = model.StartDateTime;
        var differentDateTime = newDateTime - oldDateTime;

        var oldParticipantLimit = activity.ParticipantLimit;
        var newParticipantLimit = model.ParticipantLimit;

        if(differentDateTime.TotalHours < 3){
            ModelState.AddModelError("StartDateTime", "ตั้งเวลาเริ่มกิจกรรมไม่น้อยกว่า 3 ชั่วโมงก่อนกิจกรรมเริ่ม");

            var activityTags = await _context.ActivityTags.ToListAsync();

            ViewBag.activityTags = activityTags;
            ViewBag.ActivityImageUrl = activity.ActivityImageUrl;

            return View(model);
        }

        if(newParticipantLimit < oldParticipantLimit){
            ModelState.AddModelError("ParticipantLimit", "ไม่สามารถตั้งจำนวนผู้เข้าร่วมน้อยกว่าเดิมได้");

            var activityTags = await _context.ActivityTags.ToListAsync();

            ViewBag.activityTags = activityTags;
            ViewBag.ActivityImageUrl = activity.ActivityImageUrl;

            return View(model);
        }

        DateOnly startDate = DateOnly.FromDateTime(model.StartDateTime);
        TimeSpan startTime = model.StartDateTime.TimeOfDay;

        activity.ActivityTagId = model.ActivityTag;
        activity.ActivityName = model.ActivityName;
        activity.Location = model.Location;
        activity.StartDate = startDate;
        activity.StartTime = startTime;
        activity.Detail = model.Detail;
        activity.ParticipantLimit = model.ParticipantLimit;

        if (file != null && file.Length != 0)
        {
            Console.WriteLine("test upload image to cloudinary");

            var uploadResult = await _cloudinaryService.UploadImageAsync(file);
            if (uploadResult == null)
            {
                Console.WriteLine("อัปโหลดรูปภาพไปยัง Cloudinary ไม่สำเร็จ");
                ModelState.AddModelError("", "อัปโหลดรูปภาพไปยัง Cloudinary ไม่สำเร็จ");

                return View(model);
            }

            Console.WriteLine("อัพโหลดรูปภาพไปยัง Cloudinary สำเร็จ");

            activity.ActivityImageUrl = uploadResult.SecureUrl.AbsoluteUri;
        }

        _context.Activities.Update(activity);
        await _context.SaveChangesAsync();

        return RedirectToAction("Detail", "Activity", new { id = activityIdHash });
    }
}