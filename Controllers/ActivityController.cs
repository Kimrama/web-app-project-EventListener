using Microsoft.AspNetCore.Mvc;

namespace EventListener.Controllers;

public class ActivityController : Controller
{
    public IActionResult Detail()
    {
        return View();
    }

    // Create
    public IActionResult Create()
    {
        return View();
    }
}