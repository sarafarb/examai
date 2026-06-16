namespace ExamAI.Identity.API.Events;

public record EmailVerifiedEvent(Guid UserId, string Email);

public record VerificationResentEvent(Guid UserId, string Email, string RawVerificationToken);