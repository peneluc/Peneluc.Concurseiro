using Microsoft.AspNetCore.Mvc;
using Peneluc.Concurseiro.Web.ViewModels;
using System.Net.Http.Json;

namespace Peneluc.Concurseiro.Web.Controllers;

public class QuestionsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public QuestionsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index(
        string? subjectId,
        string? difficulty,
        int page = 1)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var url =
            $"questions?page={page}&pageSize=10" +
            $"&subjectId={subjectId}&difficulty={difficulty}";

        var response = await client.GetFromJsonAsync<
            PagedResultViewModel<QuestionViewModel>>(url);

        response ??= new PagedResultViewModel<QuestionViewModel>
        {
            Data = new List<QuestionViewModel>(),
            Page = page,
            PageSize = 10,
            TotalRecords = 0
        };

        return View(response);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var question = await client.GetFromJsonAsync<QuestionViewModel>(
            $"questions/{id}");

        if (question == null)
            return NotFound();

        return View(question);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateQuestionViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = _httpClientFactory.CreateClient("ApiClient");

        var response = await client.PostAsJsonAsync("questions", model);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Erro ao criar questão.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var question = await client.GetFromJsonAsync<QuestionViewModel>(
            $"questions/{id}");

        if (question == null)
            return NotFound();

        var updateVm = new UpdateQuestionViewModel
        {
            Id = question.Id,
            Statement = question.Statement,
            Difficulty = question.Difficulty,
            SubjectId = question.SubjectId,
            ExamSourceId = question.ExamSourceId,
            Explanation = question.Explanation
        };

        return View(updateVm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateQuestionViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = _httpClientFactory.CreateClient("ApiClient");

        var response = await client.PutAsJsonAsync($"questions/{id}", model);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Erro ao atualizar.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(Guid id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var question = await client.GetFromJsonAsync<QuestionViewModel>(
            $"questions/{id}");

        if (question == null)
            return NotFound();

        return View(question);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        await client.DeleteAsync($"questions/{id}");

        return RedirectToAction(nameof(Index));
    }
}
