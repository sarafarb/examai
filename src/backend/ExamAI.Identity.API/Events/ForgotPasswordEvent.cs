namespace ExamAI.Identity.API.Events;

public record ForgotPasswordRequestedEvent(Guid UserId, string Email, string ResetToken);