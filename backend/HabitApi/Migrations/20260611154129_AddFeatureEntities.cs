using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HabitApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFeatureEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    EarnedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HabitTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Icon = table.Column<string>(type: "text", nullable: false),
                    IsPositive = table.Column<bool>(type: "boolean", nullable: false),
                    HasPenalty = table.Column<bool>(type: "boolean", nullable: false),
                    TriggerType = table.Column<int>(type: "integer", nullable: false),
                    TriggerValue = table.Column<int>(type: "integer", nullable: false),
                    TargetDays = table.Column<int>(type: "integer", nullable: false),
                    InstallCount = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsSystem = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HabitTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Author = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Streaks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    HabitId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentStreak = table.Column<int>(type: "integer", nullable: false),
                    LongestStreak = table.Column<int>(type: "integer", nullable: false),
                    LastCompletedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Streaks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLevels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    XP = table.Column<int>(type: "integer", nullable: false),
                    Level = table.Column<int>(type: "integer", nullable: false),
                    NextLevelXP = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLevels", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "HabitTemplates",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "HasPenalty", "Icon", "InstallCount", "IsActive", "IsPositive", "IsSystem", "Name", "TargetDays", "TriggerType", "TriggerValue" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111101"), "Mindfulness", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(4195), "10 minutes of mindfulness meditation", false, "🧘", 0, true, true, true, "Morning Meditation", 30, 1, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111102"), "Learning", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(7697), "Read a book for 30 minutes daily", false, "📚", 0, true, true, true, "Read 30 Minutes", 30, 1, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111103"), "Health", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(7710), "Stay hydrated throughout the day", false, "💧", 0, true, true, true, "Drink 8 Glasses Water", 30, 2, 8 },
                    { new Guid("11111111-1111-1111-1111-111111111104"), "Fitness", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(7712), "30 minutes of exercise", false, "💪", 0, true, true, true, "Workout", 30, 1, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111105"), "Mindfulness", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(7715), "Write 3 things you are grateful for", false, "📝", 0, true, true, true, "Gratitude Journal", 30, 1, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111106"), "Productivity", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(7717), "Avoid social media for the day", false, "📵", 0, true, false, true, "No Social Media", 30, 1, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111107"), "Health", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(7718), "Get a full night of quality sleep", false, "😴", 0, true, true, true, "Sleep 8 Hours", 30, 1, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111108"), "Learning", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(7720), "30 minutes of coding practice", false, "💻", 0, true, true, true, "Learn Coding", 30, 1, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111109"), "Health", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(7722), "Avoid added sugar for the day", false, "🚫", 0, true, false, true, "No Sugar", 30, 1, 1 },
                    { new Guid("11111111-1111-1111-1111-111111111110"), "Mindfulness", new DateTime(2026, 6, 11, 15, 41, 28, 208, DateTimeKind.Utc).AddTicks(7724), "5 minutes of deep breathing exercises", false, "🌬️", 0, true, true, true, "Deep Breathing", 30, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "Quotes",
                columns: new[] { "Id", "Author", "Category", "IsActive", "Text" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222201"), "Aristotle", "discipline", true, "We are what we repeatedly do. Excellence, then, is not an act, but a habit." },
                    { new Guid("22222222-2222-2222-2222-222222222202"), "Mark Twain", "motivation", true, "The secret of getting ahead is getting started." },
                    { new Guid("22222222-2222-2222-2222-222222222203"), "Jim Ryun", "motivation", true, "Motivation is what gets you started. Habit is what keeps you going." },
                    { new Guid("22222222-2222-2222-2222-222222222204"), "Robert Collier", "discipline", true, "Success is the sum of small efforts, repeated day in and day out." },
                    { new Guid("22222222-2222-2222-2222-222222222205"), "James Clear", "growth", true, "You do not rise to the level of your goals. You fall to the level of your systems." },
                    { new Guid("22222222-2222-2222-2222-222222222206"), "Steve Jobs", "motivation", true, "The only way to do great work is to love what you do." },
                    { new Guid("22222222-2222-2222-2222-222222222207"), "Edmund Hillary", "growth", true, "It is not the mountain we conquer, but ourselves." },
                    { new Guid("22222222-2222-2222-2222-222222222208"), "John C. Maxwell", "discipline", true, "Small disciplines repeated with consistency every day lead to great achievements." },
                    { new Guid("22222222-2222-2222-2222-222222222209"), "Chinese Proverb", "growth", true, "The best time to plant a tree was 20 years ago. The second best time is now." },
                    { new Guid("22222222-2222-2222-2222-222222222210"), "Jim Rohn", "health", true, "Take care of your body. It is the only place you have to live." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Achievements_UserId_Type",
                table: "Achievements",
                columns: new[] { "UserId", "Type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Streaks_UserId_HabitId",
                table: "Streaks",
                columns: new[] { "UserId", "HabitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLevels_UserId",
                table: "UserLevels",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "HabitTemplates");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "Streaks");

            migrationBuilder.DropTable(
                name: "UserLevels");
        }
    }
}
