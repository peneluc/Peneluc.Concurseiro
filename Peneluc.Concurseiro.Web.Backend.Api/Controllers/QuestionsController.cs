using Peneluc.Concurseiro.Web.Backend.Domain.Entities;
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

    // ===============================
    // LISTAGEM COM FILTROS E PAGINAÇÃO
    // ===============================
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
        if (pageSize <= 0) pageSize = 20;
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

    // ===============================
    // DETALHE POR ID
    // ===============================
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var question = await _repository.GetByIdAsync(id);

        if (question == null)
            return NotFound(new { message = "Questão não encontrada." });

        return Ok(question);
    }

    // ===============================
    // CRIAR
    // ===============================
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] QuestionEntity question)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await _repository.InsertAsync(question);

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            question);
    }

    // ===============================
    // ATUALIZAR
    // ===============================
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] QuestionEntity question)
    {
        if (id != question.Id)
            return BadRequest(new { message = "ID da URL diferente do corpo da requisição." });

        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Questão não encontrada." });

        await _repository.UpdateAsync(question);

        return NoContent();
    }

    // ===============================
    // REMOVER
    // ===============================
    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = "Questão não encontrada." });

        await _repository.DeleteAsync(id);

        return NoContent();
    }
}
