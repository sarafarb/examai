using Microsoft.EntityFrameworkCore;
namespace ExamAI.Shared.Infrastructure;
public abstract class BaseDbContext : DbContext { protected BaseDbContext(DbContextOptions options) : base(options) { } }
