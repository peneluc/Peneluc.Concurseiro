namespace Peneluc.Concurseiro.Web.ViewModels;

public class CreateQuestionViewModel
{
    public string Statement { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string Difficulty { get; set; } = "Easy";

    public Guid SubjectId { get; set; }
    public Guid ExamSourceId { get; set; }

    public List<CreateAnswerOptionViewModel> AnswerOptions { get; set; }
        = new();
}
