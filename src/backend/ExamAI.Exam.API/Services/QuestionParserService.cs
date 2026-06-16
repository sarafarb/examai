namespace ExamAI.Exam.API.Services
{
    public interface IQuestionParserService
    {
        Task ParseTemplateAsync(string examId, string s3Key);
    }

    public class QuestionParserService : IQuestionParserService
    {
        private readonly ILogger<QuestionParserService> _logger;

        public QuestionParserService(ILogger<QuestionParserService> logger)
        {
            _logger = logger;
        }

        public async Task ParseTemplateAsync(string examId, string s3Key)
        {
            _logger.LogInformation("Starting QuestionParser background job for Exam {ExamId}, File: {S3Key}", examId, s3Key);

            try
            {
                // 1. הורדת הקובץ מ-S3 (Mock)
                _logger.LogInformation("Downloading file from S3...");
                await Task.Delay(1000); 

                // 2. המרה של PDF לתמונות דרך ImageMagick/Pdfium (Mock)
                _logger.LogInformation("Converting PDF to images...");
                
                // 3. חילוץ טקסט עם Azure Document Intelligence (Mock)
                _logger.LogInformation("Extracting text via Azure Document Intelligence...");
                
                // 4. שליחה ל-GPT-4o לחילוץ שאלות בעברית (Mock)
                _logger.LogInformation("Sending prompt to GPT-4o: 'Extract all questions from this Hebrew exam...'");
                await Task.Delay(2000);

                // 5. שמירה ב-DB כ-JSON מובנה
                _logger.LogInformation("Parsing GPT response and inserting into exam.questions...");

                // 6. עדכון סטטוס המבחן ל-'active'
                _logger.LogInformation("Exam status updated to 'active'.");

                // 7. פרסום אירוע (Event) לשליחת מייל למורה שהבחינה מוכנה
                _logger.LogInformation("TemplateUploadedEvent published. Teacher notified.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Parser failed for Exam {ExamId}. Reverting status to 'draft' and notifying teacher.", examId);
                // במציאות: מעדכנים סטטוס חזרה ל-draft ושולחים מייל על כשל מנגנון ה-AI
            }
        }
    }
}