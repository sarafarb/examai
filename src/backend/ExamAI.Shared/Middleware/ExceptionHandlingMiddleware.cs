using Microsoft.AspNetCore.Http;
namespace ExamAI.Shared.Middleware;
public class ExceptionHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try { await next(context); }
        catch (Exception) { context.Response.StatusCode = 500; await context.Response.WriteAsJsonAsync(new { Error = "Internal Server Error" }); }
    }
}
