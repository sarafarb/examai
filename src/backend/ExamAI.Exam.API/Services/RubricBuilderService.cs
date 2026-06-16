namespace ExamAI.Exam.API.Services
{
    public interface IRubricBuilderService
    {
        Task BuildRubricAsync(string examId, string s3Key);
    }

    public class RubricBuilderService : IRubricBuilderService
    {
        private readonly ILogger<RubricBuilderService> _logger;

        public RubricBuilderService(ILogger<RubricBuilderService> logger)
        {
            _logger = logger;
        }

        public async Task BuildRubricAsync(string examId, string s3Key)
        {
            _logger.LogInformation("Starting RubricBuilder background job for Exam {ExamId}, File: {S3Key}", examId, s3Key);

            try
            {
                // 1. הורדת הקובץ מ-S3 (Mock)
                await Task.Delay(500); 

                // 2. חילוץ טקסט מבוסס OCR (Mock)
                _logger.LogInformation("Extracting text from answer key...");

                // 3. שליחה ל-GPT-4o מול השאלות הקיימות ליצירת מחוון (Mock)
                _logger.LogInformation("Sending prompt to GPT-4o to build a grading rubric...");
                await Task.Delay(2000);

                // 4. התאמת (Matching) בין המחוון למספרי השאלות שנשמרו קודם
                _logger.LogInformation("Matching rubric items to questions by questionNumber...");

                // 5. הזרקת הנתונים ל-exam.rubric_items
                _logger.LogInformation("Inserting items into exam.rubric_items.");

                // 6. פרסום אירוע
                _logger.LogInformation("RubricBuiltEvent published.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rubric builder failed for Exam {ExamId}.", examId);
            }
        }
    }
}