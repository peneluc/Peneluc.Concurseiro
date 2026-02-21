namespace Peneluc.Concurseiro.Web.ViewModels;

public class UpdateQuestionViewModel
{
    public Guid Id { get; set; }

    public string Statement { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public string Difficulty { get; set; } = string.Empty;

    public Guid SubjectId { get; set; }
    public Guid ExamSourceId { get; set; }

    public List<UpdateAnswerOptionViewModel> AnswerOptions { get; set; }
        = new();
}
