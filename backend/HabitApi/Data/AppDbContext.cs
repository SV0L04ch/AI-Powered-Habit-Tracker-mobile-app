using HabitApi.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Data;

public sealed class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitEntry> HabitEntries { get; set; }
    public DbSet<Streak> Streaks { get; set; }
    public DbSet<Achievement> Achievements { get; set; }
    public DbSet<UserLevel> UserLevels { get; set; }
    public DbSet<HabitTemplate> HabitTemplates { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<HabitSchedule> HabitSchedules { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<MoodEntry> MoodEntries { get; set; }
    public DbSet<HabitNote> HabitNotes { get; set; }
    public DbSet<HabitPhoto> HabitPhotos { get; set; }
    public DbSet<HabitLocation> HabitLocations { get; set; }
    public DbSet<SocialFeed> SocialFeed { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Challenge> Challenges { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<SleepEntry> SleepEntries { get; set; }
    public DbSet<MealEntry> MealEntries { get; set; }
    public DbSet<Webhook> Webhooks { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<ChallengeParticipant> ChallengeParticipants { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Игнорируем таблицы ролей и связанные
        modelBuilder.Ignore<IdentityRole<Guid>>();
        modelBuilder.Ignore<IdentityRoleClaim<Guid>>();
        modelBuilder.Ignore<IdentityUserRole<Guid>>();
        modelBuilder.Ignore<IdentityUserClaim<Guid>>();
        modelBuilder.Ignore<IdentityUserLogin<Guid>>();

        // Habit
        modelBuilder.Entity<Habit>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.HasOne(h => h.User)
                  .WithMany(u => u.Habits)
                  .HasForeignKey(h => h.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
            entity.Property(h => h.IsPositive).IsRequired();
            entity.Property(h => h.HasPenalty).IsRequired();
            entity.Property(h => h.TriggerType).IsRequired().HasConversion<string>();
            entity.Property(h => h.TriggerValue).IsRequired().HasMaxLength(10);
            entity.Property(h => h.TargetDays).HasDefaultValue(30);
            entity.Property(h => h.PenaltyDaysPerMiss).HasDefaultValue(0);
            entity.Property(h => h.Reminders).HasColumnType("jsonb");
            entity.HasIndex(h => h.UserId);
        });

        //HabitEntry
        modelBuilder.Entity<HabitEntry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Habit)
                  .WithMany(h => h.Entries)
                  .HasForeignKey(e => e.HabitId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.HasIndex(e => new { e.HabitId, e.Date }).IsUnique();
        });

        // Streak
        modelBuilder.Entity<Streak>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => new { s.UserId, s.HabitId }).IsUnique();
        });

        // Achievement
        modelBuilder.Entity<Achievement>(entity =>
        {
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => new { a.UserId, a.Type }).IsUnique();
        });

        // UserLevel
        modelBuilder.Entity<UserLevel>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.UserId).IsUnique();
        });

        // HabitTemplate
        modelBuilder.Entity<HabitTemplate>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(200);
            entity.Property(t => t.Category).IsRequired().HasMaxLength(100);
        });

        // Quote
        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(q => q.Id);
            entity.Property(q => q.Text).IsRequired().HasMaxLength(1000);
            entity.Property(q => q.Author).HasMaxLength(200);
            entity.Property(q => q.Category).HasMaxLength(100);
        });

        // HabitSchedule
        modelBuilder.Entity<HabitSchedule>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => s.HabitId).IsUnique();
            entity.Property(s => s.Frequency).IsRequired().HasMaxLength(50);
            entity.Property(s => s.DaysOfWeek).HasColumnType("jsonb");
            entity.Property(s => s.Exceptions).HasColumnType("jsonb");
        });

        // Wallet
        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.HasIndex(w => w.UserId).IsUnique();
        });

        // Transaction
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => t.UserId);
            entity.Property(t => t.Type).IsRequired().HasMaxLength(50);
            entity.Property(t => t.Description).HasMaxLength(500);
        });

        // Habit Color
        modelBuilder.Entity<Habit>(entity =>
        {
            entity.Property(h => h.Color).HasMaxLength(7);
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Status).IsRequired().HasMaxLength(20);
            entity.HasIndex(f => new { f.UserId, f.FriendId }).IsUnique();
        });

        modelBuilder.Entity<Challenge>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(200);
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.HasIndex(c => c.IsActive);
        });

        modelBuilder.Entity<ChallengeParticipant>(entity =>
        {
            entity.HasKey(cp => cp.Id);
            entity.HasIndex(cp => new { cp.ChallengeId, cp.UserId }).IsUnique();
        });

        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Title).IsRequired().HasMaxLength(200);
            entity.HasIndex(g => g.UserId);
        });

        modelBuilder.Entity<HabitPhoto>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.PhotoUrl).IsRequired().HasMaxLength(2000);
            entity.Property(p => p.Caption).HasMaxLength(500);
            entity.HasIndex(p => p.HabitEntryId);
        });

        modelBuilder.Entity<HabitLocation>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Name).HasMaxLength(200);
            entity.HasIndex(l => l.HabitEntryId);
        });

        modelBuilder.Entity<League>(entity =>
        {
            entity.HasKey(l => l.Id);
            entity.Property(l => l.Name).IsRequired().HasMaxLength(100);
            entity.Property(l => l.Tier).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Webhook>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.Property(w => w.Url).IsRequired().HasMaxLength(2000);
            entity.Property(w => w.Secret).HasMaxLength(200);
            entity.HasIndex(w => w.UserId);
        });

        modelBuilder.Entity<SocialFeed>(entity =>
        {
            entity.HasKey(f => f.Id);
            entity.Property(f => f.City).IsRequired().HasMaxLength(200);
            entity.Property(f => f.HabitName).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<SleepEntry>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasIndex(s => s.UserId);
        });

        modelBuilder.Entity<MealEntry>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.Type).IsRequired().HasMaxLength(50);
            entity.Property(m => m.Foods).IsRequired().HasMaxLength(500);
            entity.HasIndex(m => m.UserId);
        });

        SeedTemplates(modelBuilder);
        SeedQuotes(modelBuilder);
    }

    private static void SeedTemplates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HabitTemplate>().HasData(
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111101"),
                Name = "Morning Meditation",
                Description = "10 minutes of mindfulness meditation",
                Category = "Mindfulness",
                Icon = "🧘",
                IsPositive = true,
                TriggerType = 1,
                TriggerValue = 1,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(1749)  // ← добавить
            },
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111102"),
                Name = "Read 30 Minutes",
                Description = "Read a book for 30 minutes daily",
                Category = "Learning",
                Icon = "📚",
                IsPositive = true,
                TriggerType = 1,
                TriggerValue = 1,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(5291)
            },
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111103"),
                Name = "Drink 8 Glasses Water",
                Description = "Stay hydrated throughout the day",
                Category = "Health",
                Icon = "💧",
                IsPositive = true,
                TriggerType = 2,
                TriggerValue = 8,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(5309)
            },
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111104"),
                Name = "Workout",
                Description = "30 minutes of exercise",
                Category = "Fitness",
                Icon = "💪",
                IsPositive = true,
                TriggerType = 1,
                TriggerValue = 1,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(5314)
            },
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111105"),
                Name = "Gratitude Journal",
                Description = "Write 3 things you are grateful for",
                Category = "Mindfulness",
                Icon = "📝",
                IsPositive = true,
                TriggerType = 1,
                TriggerValue = 1,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(5316)
            },
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111106"),
                Name = "No Social Media",
                Description = "Avoid social media for the day",
                Category = "Productivity",
                Icon = "📵",
                IsPositive = false,
                TriggerType = 1,
                TriggerValue = 1,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(5317)
            },
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111107"),
                Name = "Sleep 8 Hours",
                Description = "Get a full night of quality sleep",
                Category = "Health",
                Icon = "😴",
                IsPositive = true,
                TriggerType = 1,
                TriggerValue = 1,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(5320)
            },
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111108"),
                Name = "Learn Coding",
                Description = "30 minutes of coding practice",
                Category = "Learning",
                Icon = "💻",
                IsPositive = true,
                TriggerType = 1,
                TriggerValue = 1,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(5322)
            },
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111109"),
                Name = "No Sugar",
                Description = "Avoid added sugar for the day",
                Category = "Health",
                Icon = "🚫",
                IsPositive = false,
                TriggerType = 1,
                TriggerValue = 1,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(5324)
            },
            new HabitTemplate
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111110"),
                Name = "Deep Breathing",
                Description = "5 minutes of deep breathing exercises",
                Category = "Mindfulness",
                Icon = "🌬️",
                IsPositive = true,
                TriggerType = 1,
                TriggerValue = 1,
                TargetDays = 30,
                CreatedAt = new DateTime(2026, 6, 11, 20, 9, 12, 116, DateTimeKind.Utc).AddTicks(5326)
            }
        );
    }

    private static void SeedQuotes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Quote>().HasData(
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222201"), Text = "We are what we repeatedly do. Excellence, then, is not an act, but a habit.", Author = "Aristotle", Category = "discipline" },
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222202"), Text = "The secret of getting ahead is getting started.", Author = "Mark Twain", Category = "motivation" },
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222203"), Text = "Motivation is what gets you started. Habit is what keeps you going.", Author = "Jim Ryun", Category = "motivation" },
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222204"), Text = "Success is the sum of small efforts, repeated day in and day out.", Author = "Robert Collier", Category = "discipline" },
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222205"), Text = "You do not rise to the level of your goals. You fall to the level of your systems.", Author = "James Clear", Category = "growth" },
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222206"), Text = "The only way to do great work is to love what you do.", Author = "Steve Jobs", Category = "motivation" },
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222207"), Text = "It is not the mountain we conquer, but ourselves.", Author = "Edmund Hillary", Category = "growth" },
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222208"), Text = "Small disciplines repeated with consistency every day lead to great achievements.", Author = "John C. Maxwell", Category = "discipline" },
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222209"), Text = "The best time to plant a tree was 20 years ago. The second best time is now.", Author = "Chinese Proverb", Category = "growth" },
            new Quote { Id = Guid.Parse("22222222-2222-2222-2222-222222222210"), Text = "Take care of your body. It is the only place you have to live.", Author = "Jim Rohn", Category = "health" }
        );
    }
}
