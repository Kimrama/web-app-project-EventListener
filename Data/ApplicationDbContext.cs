using EventListener.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EventListener.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityTag> ActivityTags { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<UserInterestActivityTag> UserInterestActivityTags { get; set; }
        public DbSet<UserJoinActivity> UserJoinActivities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //User Entity
            modelBuilder.Entity<User>()
            .HasKey(u => u.UserName);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

            //Activity Entity
            modelBuilder.Entity<Activity>()
            .HasKey(a => new { a.OwnerId, a.CreatedAt });

            modelBuilder.Entity<Activity>()
            .HasIndex(a => new { a.OwnerId, a.CreatedAt })
            .IsUnique();

            modelBuilder.Entity<Activity>()
            .Property(a => a.StartTime)
            .HasColumnType("time(0)");

            modelBuilder.Entity<Activity>()
            .Property(a => a.CreatedAt)
            .HasColumnType("datetime2(0)");
            
            //ActivityTag Entity
            modelBuilder.Entity<ActivityTag>()
            .HasKey(a => a.ActivityName);

            modelBuilder.Entity<ActivityTag>()
            .HasIndex(a => a.ActivityName)
            .IsUnique();
            
            //Notification Entity
            modelBuilder.Entity<Notification>()
            .HasKey(n => n.NotificationId);

            modelBuilder.Entity<Notification>()
            .Property(n => n.ReceiveDate)
            .HasColumnType("datetime2(0)");

            //ChatMessage Entity
            modelBuilder.Entity<ChatMessage>()
            .HasKey(c => c.ChatMessageId);

            modelBuilder.Entity<ChatMessage>()
            .Property(c => c.ActivityCreatedAt)
            .HasColumnType("datetime2(0)");

            modelBuilder.Entity<ChatMessage>()
            .Property(c => c.SendDate)
            .HasColumnType("datetime2(0)");
            
            //UserInterestActivityTag Entity
            modelBuilder.Entity<UserInterestActivityTag>()
            .HasKey(u => new { u.UserId, u.ActivityTagId });

            modelBuilder.Entity<UserInterestActivityTag>()
            .HasIndex(u => new { u.UserId, u.ActivityTagId })
            .IsUnique();

            //UserJoinActivity Entity
            modelBuilder.Entity<UserJoinActivity>()
            .HasKey(u => new { u.UserId, u.ActivityOwnerId, u.ActivityCreatedAt });

            modelBuilder.Entity<UserJoinActivity>()
            .HasIndex(u => new { u.UserId, u.ActivityOwnerId, u.ActivityCreatedAt })
            .IsUnique();

            modelBuilder.Entity<UserJoinActivity>()
            .Property(u => u.ActivityCreatedAt)
            .HasColumnType("datetime2(0)");

            modelBuilder.Entity<UserJoinActivity>()
            .Property(u => u.RequestJoinDate)
            .HasColumnType("datetime2(0)");

            modelBuilder.Entity<UserJoinActivity>()
            .Property(u => u.JoinDate)
            .HasColumnType("datetime2(0)");

            //user create activity relationship
            modelBuilder.Entity<Activity>()
            .HasOne(a => a.User)
            .WithMany(b => b.Activities)
            .HasForeignKey(a => a.OwnerId);

            //user join activity relationship
            modelBuilder.Entity<UserJoinActivity>()
            .HasOne(a => a.User)
            .WithMany(b => b.UserJoinActivities)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserJoinActivity>()
            .HasOne(a => a.Activity)
            .WithMany(b => b.UserJoinActivities)
            .HasForeignKey(a => new { a.ActivityOwnerId, a.ActivityCreatedAt })
            .OnDelete(DeleteBehavior.Restrict);

            //user interest activity tag relationship
            modelBuilder.Entity<UserInterestActivityTag>()
            .HasOne(a => a.User)
            .WithMany(b => b.UserInterestActivityTags)
            .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<UserInterestActivityTag>()
            .HasOne(a => a.ActivityTag)
            .WithMany(b => b.UserInterestActivityTags)
            .HasForeignKey(a => a.ActivityTagId);

            //activity have activity tag relationship
            modelBuilder.Entity<Activity>()
            .HasOne(a => a.ActivityTag)
            .WithMany(b => b.Activities)
            .HasForeignKey(a => a.ActivityTagId);

            //user stored notification relationship
            modelBuilder.Entity<Notification>()
            .HasOne(a => a.User)
            .WithMany(b => b.Notifications)
            .HasForeignKey(a => a.UserId);

            //user send message relationship
            modelBuilder.Entity<ChatMessage>()
            .HasOne(a => a.User)
            .WithMany(b => b.ChatMessages)
            .HasForeignKey(a => a.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
            
            //activity chat message relationship
            modelBuilder.Entity<ChatMessage>()
            .HasOne(a => a.Activity)
            .WithMany(b => b.ChatMessages)
            .HasForeignKey(a => new { a.ActivityOwnerId, a.ActivityCreatedAt })
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

