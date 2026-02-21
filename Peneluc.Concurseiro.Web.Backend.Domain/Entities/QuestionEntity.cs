
namespace Peneluc.Concurseiro.Web.Backend.Domain.Entities;

public class QuestionEntity
{
    public Guid Id { get; set; }
    public string? Explanation { get; set; }

    public required string Statement { get; set; }

    public required string SubjectName { get; set; }

    public required string ExamName { get; set; }

    public required int ExamYear { get; set; }

    public required string Difficulty { get; set; }
}
