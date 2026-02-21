using Peneluc.Concurseiro.Web.Backend.Domain.Entities;
using Peneluc.Concurseiro.Web.Backend.Infrastructure.Common;

namespace Peneluc.Concurseiro.Web.Backend.Infrastructure.Interfaces;

public interface IQuestionReadRepository
{
    Task<PagedResult<QuestionEntity>> GetQuestionsAsync(
        string? subject,
        string? exam,
        string? difficulty,
        int page,
        int pageSize,
        string? orderBy);

    Task<IEnumerable<QuestionEntity>> GetAllAsync();

    Task<QuestionEntity?> GetByIdAsync(Guid id);

    Task<Guid> InsertAsync(QuestionEntity question);

    Task UpdateAsync(QuestionEntity question);

    Task DeleteAsync(Guid id);
}
