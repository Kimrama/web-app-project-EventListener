using EventListener.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EventListener.Data;
using Microsoft.EntityFrameworkCore;

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
        var activities = await _context.Activities.ToListAsync();
        return View(activities);
    }
}