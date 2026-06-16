namespace ExamAI.Identity.API.Dtos;

public record SubscriptionInfoDto(string PlanName, int PagesUsed, int PagesLimit);

public record UserProfileResponse(
    Guid Id, 
    string Email, 
    string FirstName, 
    string LastName, 
    string? AvatarUrl, 
    bool EmailVerified, 
    string[] Roles, 
    SubscriptionInfoDto Subscription
);