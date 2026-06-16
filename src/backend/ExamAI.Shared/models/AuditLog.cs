namespace ExamAI.Shared.Models;

public class AuditLog
{
    public long Id { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Insert, Update, Delete
    public string Username { get; set; } = "System";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string Changes { get; set; } = string.Empty; // JSON של השינויים
}