var builder = WebApplication.CreateBuilder(args);
// 1. טעינת הגדרות Stripe מתוך ה-appsettings.json
builder.Services.Configure<ExamAI.Billing.API.Configuration.StripeSettings>(
    builder.Configuration.GetSection("StripeSettings"));

// 2. רישום השירותים החדשים במערכת
builder.Services.AddScoped<ExamAI.Billing.API.Services.IStripeClientService, ExamAI.Billing.API.Services.StripeClientService>();
builder.Services.AddScoped<ExamAI.Billing.API.Services.IUsageService, ExamAI.Billing.API.Services.UsageService>();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<ExamAI.Billing.API.Services.IInvoiceService, ExamAI.Billing.API.Services.InvoiceService>();
var app = builder.Build();

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
