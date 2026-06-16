using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace ExamAI.Shared.Logging
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(RequestDelegate _next)
        {
            this._next = _next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                await _next(context);
                stopwatch.Stop();
                
                LogRequest(context, stopwatch.ElapsedMilliseconds);
            }
            catch (System.Exception ex)
            {
                stopwatch.Stop();
                LogRequest(context, stopwatch.ElapsedMilliseconds, ex);
                throw;
            }
        }

        private void LogRequest(HttpContext context, long elapsedMilliseconds, System.Exception exception = null)
        {
            var request = context.Request;
            
            // שליפת ה-UserId מתוך ה-Claims של ה-JWT במידה וקיים
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";

            var logger = Log.ForContext("RequestMethod", request.Method)
                            .ForContext("RequestPath", request.Path)
                            .ForContext("StatusCode", context.Response.StatusCode)
                            .ForContext("DurationMs", elapsedMilliseconds)
                            .ForContext("UserId", userId);

            if (exception != null)
            {
                logger.Error(exception, "HTTP {Method} {Path} responded {StatusCode} in {Duration}ms. Error: {Message}", 
                    request.Method, request.Path, context.Response.StatusCode, elapsedMilliseconds, exception.Message);
            }
            else
            {
                logger.Information("HTTP {Method} {Path} responded {StatusCode} in {Duration}ms", 
                    request.Method, request.Path, context.Response.StatusCode, elapsedMilliseconds);
            }
        }
    }
}