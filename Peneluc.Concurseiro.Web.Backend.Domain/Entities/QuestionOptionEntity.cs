namespace Peneluc.Concurseiro.Web.Backend.Domain.Entities;

public class QuestionOptionEntity
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public string Description { get; set; }
}
