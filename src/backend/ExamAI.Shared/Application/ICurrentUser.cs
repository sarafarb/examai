namespace ExamAI.Shared.Application;
public interface ICurrentUser { string? UserId { get; } string? Email { get; } bool IsAuthenticated { get; } }
