using System.Net.WebSockets;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EventListener.Data;
using EventListener.Models;
using System.Globalization;
using System.Text.Json;
namespace EventListener.Services;

public class ChatWebSocketService
{
    private static readonly ConcurrentDictionary<string, List<WebSocket>> RoomConnections = new();
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public ChatWebSocketService(ApplicationDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }
    public string DecodeBase64(string input)
    {
        string base64 = input.Replace("-", "+").Replace("_", "/"); 
        while (base64.Length % 4 != 0)
        {
            base64 += "=";
        }
        
        var bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }  
    public async Task HandleWebSocketAsync(HttpContext context, string roomId, string userId)
    {
        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("User not found.");
            return;
        }

        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        Console.WriteLine($"✅ {user.UserName} connected to Room {roomId}");

        if (!RoomConnections.ContainsKey(roomId))
        {
            RoomConnections[roomId] = new List<WebSocket>();
        }
        RoomConnections[roomId].Add(webSocket);

        var buffer = new byte[1024 * 4];

        try
        {
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    break;
                }

                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"📩 {user.UserName} in Room {roomId}: {receivedMessage}");
                var decodePart = DecodeBase64(roomId).Split(" ", 2);
                
                var message = new ChatMessage
                {
                    SenderId = user.UserName,
                    ActivityOwnerId = decodePart[0],
                    ActivityCreatedAt = DateTime.ParseExact(decodePart[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None),
                    Message = receivedMessage,
                    SendDate = DateTime.UtcNow
                };

                _dbContext.ChatMessages.Add(message);
                await _dbContext.SaveChangesAsync();

                await BroadcastMessageAsync(roomId, $"{user.UserName}: {receivedMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ WebSocket Error: {ex.Message}");
        }
        finally
        {
            RoomConnections[roomId].Remove(webSocket);
            Console.WriteLine($"❌ {user.UserName} left Room {roomId}");
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
    }

    private async Task BroadcastMessageAsync(string roomId, string message)
    {
        if (!RoomConnections.ContainsKey(roomId)) return;
        var key = message.Split(": ", 2);
        var senderId = key[0];
        var text = key[1];

        
        var sender = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == senderId);
        var senderImageUrl = "";
        if (sender != null) {
            if(sender.UserImageUrl == null) {
                senderImageUrl = "NULL";
            } else {
                senderImageUrl = sender.UserImageUrl;
            }
        }
        var chatMessage = new 
        {
            SenderId = senderId,
            senderImageUrl = senderImageUrl,
            Message = text,
            
        };
        string jsonMessage = JsonSerializer.Serialize(chatMessage);

        var tasks = RoomConnections[roomId]
            .Where(ws => ws.State == WebSocketState.Open)
            .Select(async ws =>
            {
                var bytes = Encoding.UTF8.GetBytes(jsonMessage);
                await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            });

        await Task.WhenAll(tasks);
    }
}
