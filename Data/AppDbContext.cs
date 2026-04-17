using Microsoft.EntityFrameworkCore;
using HabitApi.Models.Domain;

namespace HabitApi.Data;

/// <summary>
/// Контекст базы данных для приложения трекера привычек.
/// </summary>
public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSet для всех сущностей
    public DbSet<User> Users { get; set; }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitEntry> HabitEntries { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<HabitTag> HabitTags { get; set; }
    public DbSet<WeatherData> WeatherData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========== User ==========
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.City).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Name).HasMaxLength(100);
            entity.Property(u => u.TimeZoneId).HasDefaultValue("UTC");
        });

        // ========== Habit ==========
        modelBuilder.Entity<Habit>(entity =>
        {
            entity.HasKey(h => h.Id);
            entity.HasOne(h => h.User)
                  .WithMany(u => u.Habits)
                  .HasForeignKey(h => h.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(h => h.Name).IsRequired().HasMaxLength(200);
            entity.Property(h => h.Type).IsRequired().HasConversion<string>();
            entity.Property(h => h.Category).IsRequired().HasConversion<string>();
            entity.Property(h => h.TriggerType).IsRequired().HasConversion<string>();
            entity.Property(h => h.TriggerValue).IsRequired().HasMaxLength(10);
            entity.Property(h => h.TargetDays).HasDefaultValue(30);
            entity.Property(h => h.PenaltyDaysPerMiss).HasDefaultValue(0);
            entity.Property(h => h.Reminders).HasColumnType("jsonb"); // для PostgreSQL
            entity.HasIndex(h => h.UserId);
        });

        // ========== HabitEntry ==========
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

        // ========== Tag ==========
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasIndex(t => new { t.UserId, t.Name }).IsUnique();
            entity.Property(t => t.Name).IsRequired().HasMaxLength(50);
            entity.Property(t => t.Color).HasMaxLength(7);
            entity.HasOne(t => t.User)
                  .WithMany(u => u.Tags)
                  .HasForeignKey(t => t.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ========== HabitTag (многие-ко-многим) ==========
        modelBuilder.Entity<HabitTag>(entity =>
        {
            entity.HasKey(ht => new { ht.HabitId, ht.TagId });
            entity.HasOne(ht => ht.Habit)
                  .WithMany(h => h.HabitTags)
                  .HasForeignKey(ht => ht.HabitId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ht => ht.Tag)
                  .WithMany(t => t.HabitTags)
                  .HasForeignKey(ht => ht.TagId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ========== WeatherData (кеш погоды) ==========
        modelBuilder.Entity<WeatherData>(entity =>
        {
            entity.HasKey(w => w.Id);
            entity.HasIndex(w => new { w.City, w.Date }).IsUnique();
            entity.Property(w => w.City).IsRequired().HasMaxLength(100);
            entity.Property(w => w.Date).IsRequired();
            entity.Property(w => w.Condition).HasMaxLength(50);
            entity.Property(w => w.Precipitation).HasMaxLength(100);
        });
    }
}
