namespace ExamAI.Identity.API.Dtos;

public record LoginRequest(string Email, string Password);

public record LoginResponseUser(Guid Id, string Email, string FirstName, string[] Roles);

public record LoginResponse(string AccessToken, string RefreshToken, int ExpiresIn, LoginResponseUser User);