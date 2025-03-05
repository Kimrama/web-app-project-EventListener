using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EventListener.Data;
using EventListener.Models;
using EventListener.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Azure;

namespace EventListener.Controllers;

public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly CloudinaryService _cloudinaryService;

    public ProfileController(ApplicationDbContext context, UserManager<User> userManager, CloudinaryService cloudinaryService)
    {
        _context = context;
        _userManager = userManager;
         _cloudinaryService = cloudinaryService;
    }

    [Authorize]
    public async Task<IActionResult> Index(string page)
    {
        var username = User.FindFirstValue(ClaimTypes.Name); //inspect login user

        if (string.IsNullOrEmpty(username))
        {
            return Forbid();
        }

        var user = await _userManager.FindByNameAsync(username);//fetch db ready to use

        if (user == null)
        {
            return NotFound();
        }

        var userInterestTag = await _context.UserInterestActivityTags
            .Where(u => u.UserId == username)
            .Include(u => u.ActivityTag)
            .ToListAsync();

        var model = new ProfileViewModel
        {
            Username = user.UserName,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Nickname = user.Nickname,
            SexColor = "",
            InterestTags = userInterestTag,
            UserImageUrl = user.UserImageUrl,
            About = user.About

        };
        
        var intmonth = user.Birthday.Month;
        string strmonth = null;
        switch (intmonth) 
        {
            case 1: strmonth = "January";break;
            case 2: strmonth = "Feburary";break;
            case 3: strmonth = "March";break;
            case 4: strmonth = "April";break;
            case 5: strmonth = "May";break;
            case 6: strmonth = "June";break;
            case 7: strmonth = "July";break;
            case 8: strmonth = "August";break;
            case 9: strmonth = "September";break;
            case 10: strmonth = "October";break;
            case 11: strmonth = "November";break;
            case 12: strmonth = "December";break;
        }

        model.Birthday = user.Birthday.Day.ToString() + " " + strmonth + " " + user.Birthday.Year.ToString();

        if(user.Sex == "Male"){
            model.SexColor = "#3EB5FF";
        }
        else if(user.Sex == "Female"){
            model.SexColor = "#FF87E5";
        }
        else{
            model.SexColor = "#CBC3E3";
        }
        
        int intpage = 1;
        if(page == null || Int32.Parse(page) <= 0){
            intpage = 1;
        }
        else{
            intpage = Int32.Parse(page);
        }
        var userjoinactivity = await _context.UserJoinActivities
            //Left SQL Join
            .Join(
                _context.Activities, 
                u => u.ActivityCreatedAt, 
                a => a.CreatedAt,
                (u, a) => new ProfileFullJoinModel
                    {
                        UserId = u.UserId,
                        ActivityOwnerId = u.Activity.OwnerId,
                        Status = u.Status,
                        OwnerId = a.OwnerId,
                        ActivityTagId = a.ActivityTagId,
                        ActivityName = a.ActivityName,
                        Location = a.Location,
                        StartDate = a.StartDate,
                        CreatedAt = a.CreatedAt
                    }
                )
            //Union with Right SQL Join to create full outer join table
            .Union(
                _context.Activities
                    .Where(a => _context.UserJoinActivities
                        .All(u => u.ActivityOwnerId != a.OwnerId)) // Ensures only unmatched activities
                    .Join(
                        _context.UserJoinActivities,
                        a => a.CreatedAt,
                        u => u.ActivityCreatedAt, 
                        (a, u) => new ProfileFullJoinModel
                        {
                            UserId = u.UserId,
                            ActivityOwnerId = u.Activity.OwnerId,
                            Status = u.Status,
                            OwnerId = a.OwnerId,
                            ActivityTagId = a.ActivityTagId,
                            ActivityName = a.ActivityName,
                            Location = a.Location,
                            StartDate = a.StartDate,
                            CreatedAt = a.CreatedAt
                        }
                    )
            )
            .Where(u => u.UserId == username || u.OwnerId == username)
            .Where(u => u.Status == "Accept")
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        userjoinactivity = userjoinactivity.DistinctBy(u => u.ActivityName).ToList();

        model.Numpage = Math.Ceiling((Double)userjoinactivity.Count()/(Double)4);

        userjoinactivity = await _context.UserJoinActivities
            //Left SQL Join
            .Join(
                _context.Activities, 
                u => u.ActivityCreatedAt, 
                a => a.CreatedAt,
                (u, a) => new ProfileFullJoinModel
                    {
                        UserId = u.UserId,
                        ActivityOwnerId = u.Activity.OwnerId,
                        Status = u.Status,
                        OwnerId = a.OwnerId,
                        ActivityTagId = a.ActivityTagId,
                        ActivityName = a.ActivityName,
                        Location = a.Location,
                        StartDate = a.StartDate,
                        CreatedAt = a.CreatedAt
                    }
                )
            //Union with Right SQL Join to create full outer join table
            .Union(
                _context.Activities
                    .Where(a => _context.UserJoinActivities
                        .All(u => u.ActivityOwnerId != a.OwnerId)) // Ensures only unmatched activities
                    .Join(
                        _context.UserJoinActivities,
                        a => a.CreatedAt,
                        u => u.ActivityCreatedAt, 
                        (a, u) => new ProfileFullJoinModel
                        {
                            UserId = u.UserId,
                            ActivityOwnerId = u.Activity.OwnerId,
                            Status = u.Status,
                            OwnerId = a.OwnerId,
                            ActivityTagId = a.ActivityTagId,
                            ActivityName = a.ActivityName,
                            Location = a.Location,
                            StartDate = a.StartDate,
                            CreatedAt = a.CreatedAt
                        }
                    )
            )
            .Where(u => u.UserId == username || u.OwnerId == username)
            .Where(u => u.Status == "Accept")
            .Skip(4*(intpage-1))
            .Take(4) //LIMIT (skip,take)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
        userjoinactivity = userjoinactivity.DistinctBy(u => u.ActivityName).ToList();
        model.JoinActivities = userjoinactivity;

        foreach(var act in userjoinactivity)
        {
            switch(act.ActivityTagId)
            {
                case "Football" : case "Basketball" : case "Badminton" : case "Table tennis" : case "Running" : case "Swimming" : case "Cardio" : case "Weight training" :
                    model.ActivityCategory.Add("Exercise & Sports");
                    model.ActivityCategoryColor.Add("#ff9d00");
                    break;
                case "Sing" : case "Dance" : case "Painting":
                    model.ActivityCategory.Add("Arts & Culture");
                    model.ActivityCategoryColor.Add("#04bb7b");
                    break;
                case "Gaming" : case "Board Games" : case "Food & Dining" : case "Travel":
                    model.ActivityCategory.Add("Social");
                    model.ActivityCategoryColor.Add("#b750d9");
                    break;
                case "Tutoring" : case "Lab" : case "Hackathon":
                    model.ActivityCategory.Add("Education");
                    model.ActivityCategoryColor.Add("#008cff");
                    break;
                default:
                    model.ActivityCategory.Add("Default");
                    model.ActivityCategoryColor.Add("red");
                    break;
            }
        }

        return View(model);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("Profile/Person/{usernameparam}")]
    public async Task<IActionResult> Person(string usernameparam,string page)
    {
        var username = User.FindFirstValue(ClaimTypes.Name);//inspect login user
        var user = await _userManager.FindByNameAsync(usernameparam);//fetch db ready to use

        if (user == null)
        {
            return NotFound();
        }

        if (username == usernameparam)
        {
            return RedirectToAction("Index","Profile");
        }

        var userInterestTag = await _context.UserInterestActivityTags
            .Where(u => u.UserId == usernameparam)
            .Include(u => u.ActivityTag)
            .ToListAsync();

        var model = new ProfileViewModel
        {
            Username = user.UserName,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Nickname = user.Nickname,
            SexColor = "",
            InterestTags = userInterestTag,
            UserImageUrl = user.UserImageUrl,
            About = user.About
        };

        var intmonth = user.Birthday.Month;
        string strmonth = null;
        switch (intmonth) 
        {
            case 1: strmonth = "January";break;
            case 2: strmonth = "Feburary";break;
            case 3: strmonth = "March";break;
            case 4: strmonth = "April";break;
            case 5: strmonth = "May";break;
            case 6: strmonth = "June";break;
            case 7: strmonth = "July";break;
            case 8: strmonth = "August";break;
            case 9: strmonth = "September";break;
            case 10: strmonth = "October";break;
            case 11: strmonth = "November";break;
            case 12: strmonth = "December";break;
        }

        model.Birthday = user.Birthday.Day.ToString() + " " + strmonth + " " + user.Birthday.Year.ToString();
        int currentyear = DateTime.Now.Year;
        model.Age = currentyear - Int32.Parse(user.Birthday.Year.ToString());

        if(user.Sex == "Male"){
            model.SexColor = "#3EB5FF";
        }
        else if(user.Sex == "Female"){
            model.SexColor = "#FF87E5";
        }
        else{
            model.SexColor = "#CBC3E3";
        }

        int intpage = 1;
        if(page == null || Int32.Parse(page) <= 0){
            intpage = 1;
        }
        else{
            intpage = Int32.Parse(page);
        }

       var userjoinactivity = await _context.UserJoinActivities
            //Left SQL Join
            .Join(
                _context.Activities, 
                u => u.ActivityCreatedAt, 
                a => a.CreatedAt,
                (u, a) => new ProfileFullJoinModel
                    {
                        UserId = u.UserId,
                        ActivityOwnerId = u.Activity.OwnerId,
                        Status = u.Status,
                        OwnerId = a.OwnerId,
                        ActivityTagId = a.ActivityTagId,
                        ActivityName = a.ActivityName,
                        Location = a.Location,
                        StartDate = a.StartDate,
                        CreatedAt = a.CreatedAt
                    }
                )
            //Union with Right SQL Join to create full outer join table
            .Union(
                _context.Activities
                    .Where(a => _context.UserJoinActivities
                        .All(u => u.ActivityOwnerId != a.OwnerId)) // Ensures only unmatched activities
                    .Join(
                        _context.UserJoinActivities,
                        a => a.CreatedAt,
                        u => u.ActivityCreatedAt, 
                        (a, u) => new ProfileFullJoinModel
                        {
                            UserId = u.UserId,
                            ActivityOwnerId = u.Activity.OwnerId,
                            Status = u.Status,
                            OwnerId = a.OwnerId,
                            ActivityTagId = a.ActivityTagId,
                            ActivityName = a.ActivityName,
                            Location = a.Location,
                            StartDate = a.StartDate,
                            CreatedAt = a.CreatedAt
                        }
                    )
            )
            .Where(u => u.UserId == usernameparam || u.OwnerId == usernameparam)
            .Where(u => u.Status == "Accept")
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        userjoinactivity = userjoinactivity.DistinctBy(u => u.ActivityName).ToList();

        model.Numpage = Math.Ceiling((Double)userjoinactivity.Count()/(Double)4);

        userjoinactivity = await _context.UserJoinActivities
            //Left SQL Join
            .Join(
                _context.Activities, 
                u => u.ActivityCreatedAt, 
                a => a.CreatedAt,
                (u, a) => new ProfileFullJoinModel
                    {
                        UserId = u.UserId,
                        ActivityOwnerId = u.Activity.OwnerId,
                        Status = u.Status,
                        OwnerId = a.OwnerId,
                        ActivityTagId = a.ActivityTagId,
                        ActivityName = a.ActivityName,
                        Location = a.Location,
                        StartDate = a.StartDate,
                        CreatedAt = a.CreatedAt
                    }
                )
            //Union with Right SQL Join to create full outer join table
            .Union(
                _context.Activities
                    .Where(a => _context.UserJoinActivities
                        .All(u => u.ActivityOwnerId != a.OwnerId)) // Ensures only unmatched activities
                    .Join(
                        _context.UserJoinActivities,
                        a => a.CreatedAt,
                        u => u.ActivityCreatedAt, 
                        (a, u) => new ProfileFullJoinModel
                        {
                            UserId = u.UserId,
                            ActivityOwnerId = u.Activity.OwnerId,
                            Status = u.Status,
                            OwnerId = a.OwnerId,
                            ActivityTagId = a.ActivityTagId,
                            ActivityName = a.ActivityName,
                            Location = a.Location,
                            StartDate = a.StartDate,
                            CreatedAt = a.CreatedAt
                        }
                    )
            )
            .Where(u => u.UserId == usernameparam || u.OwnerId == usernameparam)
            .Where(u => u.Status == "Accept")
            .Skip(4*(intpage-1))
            .Take(4) //LIMIT (skip,take)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        userjoinactivity = userjoinactivity.DistinctBy(u => u.ActivityName).ToList();

        model.JoinActivities = userjoinactivity;

        foreach(var act in userjoinactivity)
        {
            switch(act.ActivityTagId)
            {
                case "Football" : case "Basketball" : case "Badminton" : case "Table tennis" : case "Running" : case "Swimming" : case "Cardio" : case "Weight training" :
                    model.ActivityCategory.Add("Exercise & Sports");
                    model.ActivityCategoryColor.Add("#ff9d00");
                    break;
                case "Sing" : case "Dance" : case "Painting":
                    model.ActivityCategory.Add("Arts & Culture");
                    model.ActivityCategoryColor.Add("#04bb7b");
                    break;
                case "Gaming" : case "Board Games" : case "Food & Dining" : case "Travel":
                    model.ActivityCategory.Add("Social");
                    model.ActivityCategoryColor.Add("#b750d9");
                    break;
                case "Tutoring" : case "Lab" : case "Hackathon":
                    model.ActivityCategory.Add("Education");
                    model.ActivityCategoryColor.Add("#008cff");
                    break;
                default:
                    model.ActivityCategory.Add("Default");
                    model.ActivityCategoryColor.Add("red");
                    break;
            }
        }
        return View(model);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);

        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username is required");
        }

        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return NotFound($"User '{username}' not found.");
        }

        var tagList = await _context.ActivityTags.ToListAsync();

        var userInterestTag = await _context.UserInterestActivityTags
            .Where(u => u.UserId == username)
            .Include(u => u.ActivityTag)
            .ToListAsync();

        var model = new EditProfileViewModel
        {
            UserName = username,
            Firstname = user.Firstname,
            Lastname = user.Lastname,
            Nickname = user.Nickname,
            Birthday = user.Birthday,
            Sex = user.Sex,
            About = user.About,
            UserImageUrl = user.UserImageUrl,
            UserInterestActivityTag = userInterestTag,
            TagList = tagList
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model, string InterestTags, IFormFile file)
    {
        Console.WriteLine("test");
        var username = User.FindFirstValue(ClaimTypes.Name);
        if (string.IsNullOrEmpty(username))
        {
            return BadRequest("Username is required");
        }

        var user = await _userManager.FindByNameAsync(username);

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

            user.UserImageUrl = uploadResult.SecureUrl.AbsoluteUri;
        }

        user.Firstname = model.Firstname;
        user.Lastname = model.Lastname;
        user.Nickname = model.Nickname;
        user.Birthday = model.Birthday;
        user.Sex = model.Sex;
        user.About = model.About;

        var existingTags = await _context.UserInterestActivityTags
        .Where(t => t.UserId == username)
        .ToListAsync();
        _context.UserInterestActivityTags.RemoveRange(existingTags);

        if (!string.IsNullOrEmpty(InterestTags))
        {
            var tagsList = InterestTags.Split(',').ToList();
            var newTags = tagsList.Select(tag => new UserInterestActivityTag
            {
                UserId = username,
                ActivityTagId = tag
            })
            .ToList();

            _context.UserInterestActivityTags.AddRange(newTags);
        }

        _context.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index"); 
    }
}
