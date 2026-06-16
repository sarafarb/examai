using Microsoft.EntityFrameworkCore;

namespace ExamAI.Analytics.API.Data
{
    public class AnalyticsDbContext : DbContext
    {
        public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options)
            : base(options)
        {
        }

        // כאן בהמשך ייווצרו ה-DbSet-ים שלכם (כמו טבלאות לסטטיסטיקות, לוגים של פעולות וכו')
        // למשל:
        // public DbSet<UserActivity> UserActivities { get; set; }
    }
}