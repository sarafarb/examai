namespace ExamAI.Identity.API.Dtos;

public record RegisterRequest(string Email, string Password, string FirstName, string LastName);

public record RegisterResponse(Guid UserId, string Email, string Message);