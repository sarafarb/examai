using ExamAI.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace ExamAI.Shared.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedIdentityDataAsync(IdentityDbContext context)
    {
        // הזרקת תפקידים (Roles)
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new() { Name = "teacher" },
                new() { Name = "admin" },
                new() { Name = "super_admin" }
            };
            await context.AddRangeAsync(roles);
        }

        // הזרקת תוכניות תשלום (Plans)
        if (!await context.Plans.AnyAsync())
        {
            var plans = new List<Plan>
            {
                new() { Name = "free" },
                new() { Name = "pay_as_you_go" }
            };
            await context.AddRangeAsync(plans);
        }

        await context.SaveChangesAsync();
    }
}