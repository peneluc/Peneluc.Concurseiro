namespace Peneluc.Concurseiro.Web.ViewModels;

public class UpdateAnswerOptionViewModel
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}
