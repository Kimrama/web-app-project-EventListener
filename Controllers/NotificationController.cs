using Microsoft.AspNetCore.Mvc;

namespace EventListener.Controllers;

public class NotificationController : Controller
{
    public IActionResult Notification() 
    {
        return View();
    }
    public IActionResult Notnoti()
    {
        return View();
    }
   
}