using ExamAI.Exam.API.Services;

var builder = WebApplication.CreateBuilder(args);

// הוספת תמיכה ב-Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// רישום השירותים החדשים לפרויקט ה-Exams (T-032)
builder.Services.AddSingleton<IFileValidationService, FileValidationService>();
builder.Services.AddSingleton<IVirusScanService, VirusScanService>();
builder.Services.AddSingleton<IS3FileService, S3FileService>();

builder.Services.AddScoped<IQuestionParserService, QuestionParserService>();
builder.Services.AddScoped<IRubricBuilderService, RubricBuilderService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // הגדרות פיתוח במידת הצורך
}

app.UseAuthorization();
app.MapControllers();

app.Run();