using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace ExamAI.Shared.Logging
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private const string CorrelationIdHeaderKey = "X-Correlation-Id";

        public CorrelationIdMiddleware(RequestDelegate _next)
        {
            this._next = _next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // בדיקה אם הלקוח כבר שלח Correlation ID (למשל מה-Frontend), אם לא - נייצר חדש
            if (!context.Request.Headers.TryGetValue(CorrelationIdHeaderKey, out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            // הוספת ה-ID ל-Response Headers כדי שהלקוח יראה אותו
            context.Response.Headers[CorrelationIdHeaderKey] = correlationId;

            // דחיפת ה-ID לתוך ה-Context של Serilog (מזריק אותו אוטומטית לכל לוג שנוצר ב-Request הזה)
            using (LogContext.PushProperty("CorrelationId", correlationId.ToString()))
            {
                await _next(context);
            }
        }
    }
}