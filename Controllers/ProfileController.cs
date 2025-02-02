using Microsoft.AspNetCore.Mvc;

namespace EventListener.Controllers;

public class ProfileController : Controller 
{
    public IActionResult Me()
    {
        return View();
    }

    public IActionResult Person()
    {
        return View();
    }

    // Edit
}