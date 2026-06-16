using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ExamAI.Identity.API.Data;
using System.Security.Claims;

namespace ExamAI.Identity.API.Authorization;

// 1. Requirement המייצג הרשאה ספציפית
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }
    public PermissionRequirement(string permission) => Permission = permission;
}

// 2. Handler הבודק האם למשתמש יש את ההרשאה הנדרשת על בסיס התפקיד שלו
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionAuthorizationHandler(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // חילוץ מזהה המשתמש מהטוקן
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? context.User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
        {
            return; // לא מאומת, ה-Handler נכשל
        }

        // שימוש ב-Scope כדי לפתוח DB Context בתוך Singleton/Scoped Handler
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        // שליפת התפקידים של המשתמש מה-DB
        var userRoles = await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role.ToLower())
            .ToListAsync();

        // מיפוי לוגי מבוסס חוקים (רולים להרשאות)
        bool hasPermission = false;

        // Super Admin יכול לעשות הכל
        if (userRoles.Contains("super_admin"))
        {
            hasPermission = true;
        }
        // Admin יכול לנהל משתמשים וגישת אדמין, אך לא פעולות של סופר-אדמין ייעודיות
        else if (userRoles.Contains("admin"))
        {
            if (requirement.Permission == "admin.access" || requirement.Permission == "users.manage")
            {
                hasPermission = true;
            }
        }
        // Teacher יכול לנהל מבחנים, לבדוק אותם ולראות אנליטיקות
        else if (userRoles.Contains("teacher"))
        {
            if (requirement.Permission == "exams.write" || 
                requirement.Permission == "exams.grade" || 
                requirement.Permission == "analytics.view")
            {
                hasPermission = true;
            }
        }

        if (hasPermission)
        {
            context.Succeed(requirement); // אישור ההרשאה בהצלחה!
        }
    }
}

// 3. Extension Method לרישום נוח ב-Program.cs
public static class AuthorizationExtensions
{
    public static IServiceCollection AddExamAIAuthorization(this IServiceCollection services)
    {
        // רישום ה-Handler ב-DI
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // הגדרת הפוליסיז (Policies) כפי שנדרש במשימה
        services.AddAuthorization(opts =>
        {
            opts.AddPolicy("CanManageExams", p => p.Requirements.Add(new PermissionRequirement("exams.write")));
            opts.AddPolicy("CanGradeExams", p => p.Requirements.Add(new PermissionRequirement("exams.grade")));
            opts.AddPolicy("CanViewAnalytics", p => p.Requirements.Add(new PermissionRequirement("analytics.view")));
            opts.AddPolicy("CanAccessAdmin", p => p.Requirements.Add(new PermissionRequirement("admin.access")));
            opts.AddPolicy("CanManageUsers", p => p.Requirements.Add(new PermissionRequirement("users.manage")));
        });

        return services;
    }
}