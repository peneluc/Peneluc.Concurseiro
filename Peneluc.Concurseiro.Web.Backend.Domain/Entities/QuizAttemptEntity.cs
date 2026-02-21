namespace Peneluc.Concurseiro.Web.Backend.Domain.Entities;

public class QuizAttemptEntity
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int Score { get; set; }
}

