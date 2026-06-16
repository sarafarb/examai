namespace ExamAI.Exam.API.Models
{
    public class CreateExamRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string GradeLevel { get; set; } = string.Empty;
        public int MaxScore { get; set; } = 100;
        public int Strictness { get; set; } = 50;
    }

    public class UpdateExamRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string GradeLevel { get; set; } = string.Empty;
        public int MaxScore { get; set; } = 100;
        public int Strictness { get; set; } = 50;
    }

    public class UpdateStrictnessRequest
    {
        public int Strictness { get; set; }
    }

    public class ExamDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string GradeLevel { get; set; } = string.Empty;
        public int MaxScore { get; set; }
        public int Strictness { get; set; }
        public string Status { get; set; } = "draft";
        public int QuestionsCount { get; set; }
        public int StudentExamsCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaginatedList<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
    public class QuestionDto
    {
        public string Id { get; set; } = string.Empty;
        public int Number { get; set; }
        public string Text { get; set; } = string.Empty;
        public float MaxPoints { get; set; }
        public RubricItemDto? RubricItem { get; set; }
    }

    public class RubricItemDto
    {
        public string Id { get; set; } = string.Empty;
        public int QuestionNumber { get; set; }
        public string CorrectAnswer { get; set; } = string.Empty;
        public List<string> AlternativeAnswers { get; set; } = new();
        public string GradingNotes { get; set; } = string.Empty;
        public float Points { get; set; }
    }

    public class UpdateRubricRequest
    {
        public List<RubricItemUpdateRequest> Items { get; set; } = new();
    }

    public class RubricItemUpdateRequest
    {
        public string QuestionId { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public List<string> AlternativeAnswers { get; set; } = new();
        public string GradingNotes { get; set; } = string.Empty;
        public float Points { get; set; }
    }

    public class UpdateQuestionRequest
    {
        public string Text { get; set; } = string.Empty;
        public float MaxPoints { get; set; }
    }
}