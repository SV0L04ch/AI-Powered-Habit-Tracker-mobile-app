using HabitApi.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HabitApi.Data;

public sealed class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Habit> Habits { get; set; }
    public DbSet<HabitEntry> HabitEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // обязательно для Identity

        // ========== Habit ==========
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
    }
}
