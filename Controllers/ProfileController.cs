using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EventListener.Data;
using EventListener.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EventListener.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ProfileController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);//inspect login user

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
            About = "",
            InterestTags = userInterestTag
        };

        var intmonth = user.Birthday.Month;
        string strmonth = null;
        switch (intmonth) 
        {
            case 1:
                strmonth = "January";
                break;
            case 2:
                strmonth = "Feburary";
                break;
            case 3:
                strmonth = "March";
                break;
            case 4:
                strmonth = "April";
                break;
            case 5:
                strmonth = "May";
                break;
            case 6:
                strmonth = "June";
                break;
            case 7:
                strmonth = "July";
                break;
            case 8:
                strmonth = "August";
                break;
            case 9:
                strmonth = "September";
                break;
            case 10:
                strmonth = "October";
                break;
            case 11:
                strmonth = "November";
                break;
            case 12:
                strmonth = "December";
                break;
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

        if(user.About == null && user.Sex == "Male"){
            model.About = "<this user have not told us about himself yet>";
        }
        else if(user.About == null && user.Sex == "Female"){
            model.About = "<this user have not told us about herself yet>";
        }
        else if(user.About == null && (user.Sex != "Male" && user.Sex != "Female")){
            model.About = "<this user have not told us about themself yet>";
        }
        else{
            model.About = user.About;
        }

        return View(model);
    }

    public IActionResult Person()
    {
        return View();
    }

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
            User = user,
            UserInterestActivityTag = userInterestTag,
            TagList = tagList
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {

        // if (!ModelState.IsValid)
        // {
        //     foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        //     {
        //         Console.WriteLine($"Validation Error: {error.ErrorMessage}");
        //     }
        //     return View(model);
        // }
        string username = HttpContext.User.Identity?.Name;
        var user = await _context.Users.FindAsync(username);
        if (user == null) return NotFound();

        user.Firstname = model.User.Firstname;
        user.Lastname = model.User.Lastname;
        user.Birthday = model.User.Birthday;
        user.Sex = model.User.Sex;
        user.About = model.User.About;


        _context.Update(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Me"); 
    }
}
