
namespace Peneluc.Concurseiro.Web.Backend.Domain.Entities;

public class QuestionEntity
{
    public Guid Id { get; set; }
    public string Statement { get; set; }
    public string? Explanation { get; set; }

    public string SubjectName { get; set; }
    public string ExamName { get; set; }
    public int ExamYear { get; set; }

    public string Difficulty { get; set; }
}
