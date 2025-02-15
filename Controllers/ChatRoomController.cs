using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventListener.Data;
using EventListener.Models;
using EventListener.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace EventListener.Controllers
{
    [Authorize] // ✅ ต้องล็อกอินก่อนเข้าแชท
    public class ChatRoomController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Base64Helper _base64Helper;

        public ChatRoomController(ApplicationDbContext context, Base64Helper base64Helper)
        {
            _context = context;
            _base64Helper = base64Helper;
        }

        // ✅ เปิดหน้าห้องแชทตาม roomId (Base64)
        [HttpGet]
        [Route("Chatroom/Room/{roomId}")]
        public async Task<IActionResult> Room(string roomId)
        {
            // 🔄 Decode Base64 เป็น OwnerId + CreatedAt
            string decodedString = _base64Helper.DecodeBase64(roomId);
            string[] parts = decodedString.Split(' ', 2); // แยกเป็น OwnerId และ CreatedAt
            if (parts.Length != 2) return NotFound();
            Console.WriteLine($"user: {parts[0]}");
            Console.WriteLine($"date: {parts[1]}");
            string ownerId = parts[0];
            string createdAt = parts[1];

            // 🔎 ค้นหา Activity ตาม OwnerId และ CreatedAt
            var activity = await _context.Activities
                .Include(a => a.ChatMessages)
                .FirstOrDefaultAsync(a => a.OwnerId == ownerId && a.CreatedAt.ToString() == createdAt);

            if (activity == null)
            {
                return NotFound("Chat room not found.");
            }

            ViewData["RoomId"] = roomId; // ส่งค่าไป View
            return View(activity);
        }
    }
}
