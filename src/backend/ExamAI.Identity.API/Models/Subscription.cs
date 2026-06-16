using System.ComponentModel.DataAnnotations.Schema;

namespace ExamAI.Identity.API.Models;

[Table("subscriptions", Schema = "identity")]
public class Subscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Plan { get; set; } = "free";
    public int PagesUsed { get; set; } = 0;
    public int PagesLimit { get; set; } = 25;
}
