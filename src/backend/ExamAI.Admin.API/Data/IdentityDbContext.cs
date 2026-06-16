using Microsoft.EntityFrameworkCore;

namespace ExamAI.Shared.Data;

public class User { public int Id { get; set; } public string Email { get; set; } = ""; public string PasswordHash { get; set; } = ""; }
public class Role { public int Id { get; set; } public string Name { get; set; } = ""; }
public class Plan { public int Id { get; set; } public string Name { get; set; } = ""; }

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Plan> Plans => Set<Plan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");

        modelBuilder.Entity<User>(entity => {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique(); // אינדקס ייחודי לאימייל
        });

        modelBuilder.Entity<Role>(entity => entity.HasIndex(e => e.Name).IsUnique());
        modelBuilder.Entity<Plan>(entity => entity.HasIndex(e => e.Name).IsUnique());
    }
}