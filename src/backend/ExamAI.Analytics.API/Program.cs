using Microsoft.EntityFrameworkCore;
using ExamAI.Analytics.API.Data;
using Serilog;
using Serilog.Formatting.Json;
using ExamAI.Shared.Logging;
 // מוודא שהוא מוצא את תיקיית ה-Data של האנליטיקה
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .Enrich.WithProperty("Application", "ExamAI.Admin.API") // בפרויקט האנליטיקה שנו ל-"ExamAI.Analytics.API"
    .Destructure.With(new SensitiveDataScrubber()) // מפעיל את מנקה הסיסמאות
    .WriteTo.Console(new JsonFormatter())
    .WriteTo.Seq(ctx.Configuration["Seq:Url"] ?? "http://localhost:5341")
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AnalyticsDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=exam_db;Username=postgres;Password=postgres"));


var app = builder.Build();

// 1. קודם כל ה-Correlation ID כדי שייצר מזהה לכל הצינור
app.UseMiddleware<CorrelationIdMiddleware>();

// 2. ה-Request Logger שימדוד וידפיס את הנתונים
app.UseMiddleware<RequestLoggingMiddleware>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
