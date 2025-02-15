using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using EventListener.Data;
using EventListener.Models;
using System.Security.Claims;
using EventListener.Services;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING"))
);

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier;
});

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
});
builder.Services.AddScoped<ChatWebSocketService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // หรือ URL ของแอปที่เชื่อมต่อ
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Load Cloudinary settings Environment
builder.Configuration["Cloudinary:CloudName"] = Environment.GetEnvironmentVariable("CLOUD_NAME");
builder.Configuration["Cloudinary:ApiKey"] = Environment.GetEnvironmentVariable("API_KEY");
builder.Configuration["Cloudinary:ApiSecret"] = Environment.GetEnvironmentVariable("API_SECRET");

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddScoped<CloudinaryService>();

builder.Services.AddScoped<Base64Helper>();

var app = builder.Build();

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/ws/chat"))
    {
        var segments = context.Request.Path.Value.Split('/');
        foreach (var seg in segments) {
            Console.WriteLine(seg);
        }
        if (segments.Length == 5)
        {
            var roomId = segments[3];
            var userId = segments[4];
            var chatService = context.RequestServices.GetRequiredService<ChatWebSocketService>();
            await chatService.HandleWebSocketAsync(context, roomId, userId);
        }
    }
    else
    {
        await next();
    }
});

app.Run();