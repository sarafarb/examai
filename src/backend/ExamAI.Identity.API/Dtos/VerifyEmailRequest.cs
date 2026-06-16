namespace ExamAI.Identity.API.Dtos;

public record VerifyEmailRequest(string Token);

public record ResendVerificationRequest(string Email);