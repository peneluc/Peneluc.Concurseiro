namespace Peneluc.Concurseiro.Web.ViewModels;

public class AnswerOptionViewModel
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}
