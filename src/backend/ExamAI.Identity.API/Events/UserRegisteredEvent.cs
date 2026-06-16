namespace ExamAI.Identity.API.Events;

public record UserRegisteredEvent(Guid UserId, string Email, string RawVerificationToken);