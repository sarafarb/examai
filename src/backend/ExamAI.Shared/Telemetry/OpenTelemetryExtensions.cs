using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace ExamAI.Shared.Telemetry
{
    public static class OpenTelemetryExtensions
    {
        public static readonly ActivitySource OcrSource = new("ExamAI.OCR");
        public static readonly ActivitySource GradingSource = new("ExamAI.Grading");
        public static readonly ActivitySource ExportSource = new("ExamAI.Export");

        public static IServiceCollection AddExamTelemetry(this IServiceCollection services, string serviceName)
        {
            services.AddOpenTelemetry()
                .WithTracing(b => b
                    .ConfigureResource(r => r.AddService(serviceName)) // התיקון לשם השירות
                    .AddAspNetCoreInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation() // עכשיו זה יעבוד!
                    .AddSource("ExamAI.*")
                    .AddOtlpExporter()) // משתמש ב-Protocol החדש שהתקנו
                .WithMetrics(b => b
                    .ConfigureResource(r => r.AddService(serviceName))
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("ExamAI.Metrics")
                    .AddOtlpExporter());

            services.AddSingleton<ExamMetricsCollector>();

            return services;
        }
    }
}