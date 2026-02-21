using Peneluc.Concurseiro.Web.Backend.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Peneluc.Concurseiro.Web.Backend.Api.Controllers;

[ApiController]
[Route("api/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionReadRepository _repository;

    public QuestionsController(IQuestionReadRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestions(
        [FromQuery] string? subject,
        [FromQuery] string? exam,
        [FromQuery] string? difficulty,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? orderBy = "statement")
    {
        if (page <= 0) page = 1;
        if (pageSize > 100) pageSize = 100;

        var result = await _repository.GetQuestionsAsync(
            subject,
            exam,
            difficulty,
            page,
            pageSize,
            orderBy);

        return Ok(result);
    }
}
