using Microsoft.AspNetCore.Http;
using Serilog;
namespace ExamAI.Shared.Middleware;
public class RequestLoggingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        Log.Information("Handling request: {Path}", context.Request.Path);
        await next(context);
    }
}
