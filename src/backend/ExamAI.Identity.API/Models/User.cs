using System.ComponentModel.DataAnnotations.Schema;

namespace ExamAI.Identity.API.Models;

[Table("users", Schema = "identity")]
public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; } 
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool EmailVerified { get; set; } = false;
    public bool IsDeleted { get; set; } = false;
    public bool IsSuspended { get; set; } = false;
    public string? GoogleId { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}