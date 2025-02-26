using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventListener.Data;
using EventListener.Models;
using EventListener.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EventListener.Controllers
{
    [Authorize] 
    public class ChatRoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChatRoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Detail() {
            var activities = await _context.Activities.Select(a => new ChatroomListFromActivity {
                ActivityName = a.ActivityName,
                ActivityOwnerId = a.OwnerId,
                ActivityCreateAt = a.CreatedAt,
                RoomIdHash = Base64Helper.EncodeBase64(a.OwnerId + " " + a.CreatedAt.ToString())
            }).ToListAsync();

            return View(activities);
        }

        [HttpGet]
        [Route("Chatroom/Room/{roomId}")]
        public async Task<IActionResult> Room(string roomId)
        {
            string decodedString = Base64Helper.DecodeBase64(roomId);
            string[] parts = decodedString.Split(' ', 2); 
            if (parts.Length != 2) return NotFound();
            Console.WriteLine($"user: {parts[0]}");
            Console.WriteLine($"date: {parts[1]}");
            string ownerId = parts[0];
            string createdAt = parts[1];

            var activity = await _context.Activities
                .Include(a => a.ChatMessages)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(a => a.OwnerId == ownerId && a.CreatedAt.ToString() == createdAt);

            if (activity == null)
            {
                return NotFound("Chat room not found.");
            }

            ViewData["RoomId"] = roomId;
            return View(activity);
        }
    }
}
