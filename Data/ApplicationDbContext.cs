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
        public DbSet<UserJoinActivity> UserJoinActivities { get; set; }
        public DbSet<UserInterestActivityTag> UserInterestActivityTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .HasKey(u => u.UserName);

            modelBuilder.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();

            modelBuilder.Entity<Activity>()
            .HasKey(a => new { a.OwnerId, a.CreatedAt });

            modelBuilder.Entity<Activity>()
            .HasIndex(a => new { a.OwnerId, a.CreatedAt })
            .IsUnique();

            modelBuilder.Entity<ActivityTag>()
            .HasKey(a => a.Name);

            modelBuilder.Entity<ActivityTag>()
            .HasIndex(a => a.Name)
            .IsUnique();

            modelBuilder.Entity<Notification>()
            .HasKey(n => n.NotificationId);

            modelBuilder.Entity<ChatMessage>()
            .HasKey(c => c.ChatMessageId);

            modelBuilder.Entity<UserJoinActivity>()
            .HasKey(uja => new { uja.UserId, uja.ActivityOwnerId, uja.ActivityCreatedAt });

            modelBuilder.Entity<UserJoinActivity>()
            .HasIndex(uja => new { uja.UserId, uja.ActivityOwnerId, uja.ActivityCreatedAt })
            .IsUnique();

            modelBuilder.Entity<UserInterestActivityTag>()
            .HasKey(u => new { u.UserId, u.ActivityTagId });

            modelBuilder.Entity<UserInterestActivityTag>()
            .HasIndex(u => new { u.UserId, u.ActivityTagId })
            .IsUnique();

            modelBuilder.Entity<Activity>()
            .HasOne(a => a.User)
            .WithMany(b => b.Activities)
            .HasForeignKey(a => a.OwnerId);

            modelBuilder.Entity<UserJoinActivity>()
            .HasOne(a => a.User)
            .WithMany(b => b.UserJoinActivities)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserJoinActivity>()
            .HasOne(a => a.Activity)
            .WithMany(b => b.UserJoinActivities)
            .HasForeignKey(a => new { a.ActivityOwnerId, a.ActivityCreatedAt })
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserInterestActivityTag>()
            .HasOne(a => a.User)
            .WithMany(b => b.UserInterestActivityTags)
            .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<UserInterestActivityTag>()
            .HasOne(a => a.ActivityTag)
            .WithMany(b => b.UserInterestActivityTags)
            .HasForeignKey(a => a.ActivityTagId);

            modelBuilder.Entity<Activity>()
            .HasOne(a => a.ActivityTag)
            .WithMany(b => b.Activities)
            .HasForeignKey(a => a.ActivityTagId);

            modelBuilder.Entity<Notification>()
            .HasOne(a => a.User)
            .WithMany(b => b.Notifications)
            .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<ChatMessage>()
            .HasOne(a => a.User)
            .WithMany(b => b.ChatMessages)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatMessage>()
            .HasOne(a => a.Activity)
            .WithMany(b => b.ChatMessages)
            .HasForeignKey(a => new { a.ActivityOwnerId, a.ActivityCreatedAt })
            .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

