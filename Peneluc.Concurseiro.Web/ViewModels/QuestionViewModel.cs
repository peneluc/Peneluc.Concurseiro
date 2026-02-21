namespace Peneluc.Concurseiro.Web.ViewModels;

public class QuestionViewModel
{
    public Guid Id { get; set; }

    public string Statement { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string Difficulty { get; set; } = string.Empty;

    public Guid SubjectId { get; set; }
    public string? SubjectName { get; set; }

    public Guid ExamSourceId { get; set; }
    public string? ExamName { get; set; }
    public int ExamYear { get; set; }

    public List<AnswerOptionViewModel> AnswerOptions { get; set; }
        = new();
}
