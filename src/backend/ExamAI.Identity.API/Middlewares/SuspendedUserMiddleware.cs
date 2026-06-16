using System.Security.Claims;
using ExamAI.Identity.API.Data;

namespace ExamAI.Identity.API.Middlewares;

public class SuspendedUserMiddleware
{
    private readonly RequestDelegate _next;

    public SuspendedUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IdentityDbContext dbContext)
    {
        // חילוץ ה-UserId מתוך ה-Claims (אם קיים בבקשה הנוכחית)
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? context.User.FindFirst("sub")?.Value;

        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out Guid userId))
        {
            // בדיקה האם המשתמש מושעה ב-DB
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null && user.IsSuspended)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { error = "Your account has been suspended." });
                return; // עוצרים את השרשרת ולא מעבירים את הבקשה הלאה!
            }
        }

        await _next(context); // המשתמש תקין או לא רשום, ממשיכים כרגיל
    }
}