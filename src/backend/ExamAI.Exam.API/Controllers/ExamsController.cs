using Microsoft.AspNetCore.Mvc;
using ExamAI.Exam.API.Models;
using ExamAI.Exam.API.Services;

namespace ExamAI.Exam.API.Controllers
{
    [ApiController]
    [Route("api/v1/exams")]
    public class ExamsController : ControllerBase
    {
        private readonly IS3FileService _s3FileService;
        private readonly IVirusScanService _virusScanService;
        private readonly IFileValidationService _fileValidationService;
        private readonly IServiceScopeFactory _scopeFactory;

        public ExamsController(
            IS3FileService s3FileService, 
            IVirusScanService virusScanService, 
            IFileValidationService fileValidationService,
            IServiceScopeFactory scopeFactory)
        {
            _s3FileService = s3FileService;
            _virusScanService = virusScanService;
            _fileValidationService = fileValidationService;
            _scopeFactory = scopeFactory;
        }

        // --- T-034: Template Upload & Trigger Parser ---
        [HttpPost("{id}/template")]
        public async Task<IActionResult> UploadTemplate(string id, IFormFile file)
        {
            var (isSuccess, s3Key, errorResponse) = await ProcessAndUploadFileAsync(id, file, "template");
            if (!isSuccess) return errorResponse!;

            // טריגר לתהליך הניתוח ברקע (Fire and Forget)
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var parserService = scope.ServiceProvider.GetRequiredService<IQuestionParserService>();
                await parserService.ParseTemplateAsync(id, s3Key!);
            });

            return StatusCode(201, new { Message = "Template uploaded successfully. Parsing started in background." });
        }

        // --- T-035: Answer Key Upload & Trigger Rubric ---
        [HttpPost("{id}/answer-key")]
        public async Task<IActionResult> UploadAnswerKey(string id, IFormFile file)
        {
            var (isSuccess, s3Key, errorResponse) = await ProcessAndUploadFileAsync(id, file, "answer_key");
            if (!isSuccess) return errorResponse!;

            // טריגר לתהליך המחוון ברקע
            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var rubricService = scope.ServiceProvider.GetRequiredService<IRubricBuilderService>();
                await rubricService.BuildRubricAsync(id, s3Key!);
            });

            return StatusCode(201, new { Message = "Answer key uploaded successfully. Rubric build started in background." });
        }

        // --- T-034: Get Questions ---
        [HttpGet("{id}/questions")]
        public IActionResult GetQuestions(string id)
        {
            var mockQuestions = new List<QuestionDto>
            {
                new QuestionDto { Id = Guid.NewGuid().ToString(), Number = 1, Text = "מהי בירת צרפת?", MaxPoints = 10 }
            };
            return Ok(mockQuestions);
        }

        // --- T-035: Get Rubric ---
        [HttpGet("{id}/rubric")]
        public IActionResult GetRubric(string id)
        {
            var mockRubric = new List<QuestionDto>
            {
                new QuestionDto 
                { 
                    Id = Guid.NewGuid().ToString(), Number = 1, Text = "מהי בירת צרפת?", MaxPoints = 10,
                    RubricItem = new RubricItemDto { Id = Guid.NewGuid().ToString(), QuestionNumber = 1, CorrectAnswer = "פריז", Points = 10 }
                }
            };
            return Ok(mockRubric);
        }

        // --- T-035: Update Rubric ---
        [HttpPut("{id}/rubric")]
        public IActionResult UpdateRubric(string id, [FromBody] UpdateRubricRequest request)
        {
            // עדכון המחוון ב-DB ותיעוד (Audit Log)
            return Ok(new { Message = "Rubric updated successfully." });
        }

        // --- T-035: Update Question ---
        [HttpPut("{id}/questions/{questionId}")]
        public IActionResult UpdateQuestion(string id, string questionId, [FromBody] UpdateQuestionRequest request)
        {
            // עדכון השאלה הספציפית ב-DB
            return Ok(new { Message = "Question updated successfully." });
        }


        // --- פונקציית עזר פרטית לטיפול אחיד בהעלאת קבצים ---
        private async Task<(bool IsSuccess, string? S3Key, IActionResult? ErrorResponse)> ProcessAndUploadFileAsync(string examId, IFormFile file, string type)
        {
            string currentUserId = "mock-teacher-uuid";

            if (file.Length > 50 * 1024 * 1024)
                return (false, null, BadRequest(new { Error = "File size exceeds 50MB." }));

            using var stream = file.OpenReadStream();
            if (!_fileValidationService.ValidateMagicBytes(stream, file.ContentType))
                return (false, null, StatusCode(415, new { Error = "Unsupported file type." }));

            var scanResult = await _virusScanService.ScanFileAsync(stream);
            if (!scanResult.IsSafe)
                return (false, null, StatusCode(422, new { Error = $"Security threat detected: {scanResult.ThreatName}" }));

            string s3Key = await _s3FileService.UploadFileAsync(stream, file.FileName, file.ContentType, currentUserId, examId, type);
            return (true, s3Key, null);
        }

        // === מתודות ה-CRUD הקודמות (משימה T-033) נשארו כאן בקצרה כדי לא לאבד אותן ===
        [HttpPost]
        public IActionResult CreateExam([FromBody] CreateExamRequest request) => StatusCode(201, new ExamDto { Id = Guid.NewGuid().ToString(), Status = "draft" });

        [HttpGet]
        public IActionResult GetExams([FromQuery] int page = 1) { _ = page; return Ok(new PaginatedList<ExamDto>()); }

        [HttpGet("{id}")]
        public IActionResult GetExamById(string id) => Ok(new ExamDto { Id = id });

        [HttpPut("{id}")]
        public IActionResult UpdateExam(string id, [FromBody] UpdateExamRequest request) => Ok();

        [HttpDelete("{id}")]
        public IActionResult DeleteExam(string id) => NoContent();

        [HttpPut("{id}/strictness")]
        public IActionResult UpdateStrictness(string id, [FromBody] UpdateStrictnessRequest request) => Ok();
    }
}