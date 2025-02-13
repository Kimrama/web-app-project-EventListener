using System.Globalization;
using System.Security.Claims;
using System.Xml.Schema;
using EventListener.Data;
using EventListener.Models;
using EventListener.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventListener.Controllers;

public class ActivityController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly CloudinaryService _cloudinaryService;

    public ActivityController(ApplicationDbContext context, UserManager<User> userManager, CloudinaryService cloudinaryService)
    {
        _context = context;
        _userManager = userManager;
        _cloudinaryService = cloudinaryService;
    }

    public string EncodeBase64(string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytes);
    }

    public string DecodeBase64(string input)
    {
        string decodedInput = Uri.UnescapeDataString(input); 
        var bytes = Convert.FromBase64String(decodedInput);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    [HttpGet]
    [Route("Activity/Detail/{activityIdHash}")]
    public async Task<IActionResult> Detail(string activityIdHash)
    {
        Console.WriteLine(activityIdHash);

        var activityId = DecodeBase64(activityIdHash);
        Console.WriteLine(activityId);

        var keys = activityId.Split(" ", 2);
        Console.WriteLine(keys[0]);
        Console.WriteLine(keys[1]);

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

        var model = new ActivityDetailViewModel
        {
            Activity = activity,
            UserJoinActivity = usersJoinActivity,
            UserJoinActivityCount = userJoinActivityCount
        };

        return View(model);
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
            ActivityImageUrl = uploadResult.SecureUrl.AbsoluteUri
        };

        _context.Activities.Add(activity);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home");
    }
}
