using System.ComponentModel.DataAnnotations.Schema;

namespace ExamAI.Identity.API.Models;

[Table("email_verifications", Schema = "identity")]
public class EmailVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UsedAt { get; set; }
}