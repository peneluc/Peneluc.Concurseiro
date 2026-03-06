﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Peneluc.Concurseiro.Web.ViewModels;
using System.Linq;
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
        string? searchTerm,
        string? sortOrder,
        DateTime? startDate,
        DateTime? endDate,
        int page = 1)
    {
        // Carrega os dados para o dropdown de matérias e dificuldades
        await LoadSelectLists(subjectId);
        var difficultyLevels = new List<SelectListItem>
        {
            new() { Value = "Easy", Text = "Easy" },
            new() { Value = "Medium", Text = "Medium" },
            new() { Value = "Hard", Text = "Hard" },
        };
        ViewBag.Difficulties = new SelectList(difficultyLevels, "Value", "Text", difficulty);

        // Lógica de ordenação para a View (alterna entre asc/desc)
        ViewBag.StatementSortParm = sortOrder == "statement" ? "statement_desc" : "statement";
        ViewBag.DifficultySortParm = sortOrder == "difficulty" ? "difficulty_desc" : "difficulty";
        ViewBag.SubjectSortParm = sortOrder == "subject" ? "subject_desc" : "subject";
        ViewBag.ExamSortParm = sortOrder == "exam" ? "exam_desc" : "exam";
        ViewBag.DateSortParm = sortOrder == "date" ? "date_desc" : "date";
        ViewBag.CurrentSortOrder = sortOrder;

        var client = _httpClientFactory.CreateClient("ApiClient");

        var url =
            $"questions?page={page}&pageSize=10" +
            $"&subjectId={subjectId}&difficulty={difficulty}&searchTerm={searchTerm}&sortOrder={sortOrder}" +
            $"&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

        var response = await client.GetFromJsonAsync<
            PagedResultViewModel<QuestionViewModel>>(url);

        response ??= new PagedResultViewModel<QuestionViewModel>
        {
            Data = new List<QuestionViewModel>(),
            Page = page,
            PageSize = 10,
            TotalRecords = 0
        };

        // Passa os filtros atuais para a View, para manter o estado nos links de paginação
        ViewBag.CurrentSubjectId = subjectId;
        ViewBag.CurrentDifficulty = difficulty;
        ViewBag.CurrentSearchTerm = searchTerm;
        ViewBag.CurrentStartDate = startDate?.ToString("yyyy-MM-dd");
        ViewBag.CurrentEndDate = endDate?.ToString("yyyy-MM-dd");

        return View(response);
    }

    public async Task<IActionResult> ExportCsv(
        string? subjectId,
        string? difficulty,
        string? searchTerm,
        string? sortOrder,
        DateTime? startDate,
        DateTime? endDate)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        // Solicita uma página grande para exportar todos os registros filtrados
        var url =
            $"questions?page=1&pageSize=10000" +
            $"&subjectId={subjectId}&difficulty={difficulty}&searchTerm={searchTerm}&sortOrder={sortOrder}" +
            $"&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";

        var response = await client.GetFromJsonAsync<
            PagedResultViewModel<QuestionViewModel>>(url);

        var data = response?.Data ?? new List<QuestionViewModel>();

        var builder = new System.Text.StringBuilder();
        builder.AppendLine("Id;Enunciado;Materia;Prova;Ano;Dificuldade;DataCriacao");

        foreach (var item in data)
        {
            builder.AppendLine($"{item.Id};\"{item.Statement?.Replace("\"", "\"\"")}\";\"{item.SubjectName}\";\"{item.ExamName}\";{item.ExamYear};{item.Difficulty};{item.CreatedAt:dd/MM/yyyy}");
        }

        return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "questoes.csv");
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

    public async Task<IActionResult> Create()
    {
        await LoadSelectLists(null, null);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateQuestionViewModel model)
    {
        if (!ModelState.IsValid) 
        {
            await LoadSelectLists();
            return View(model);
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        var response = await client.PostAsJsonAsync("questions", model);

        if (!response.IsSuccessStatusCode)
        {
            await HandleApiFailure(response);
            await LoadSelectLists(model.SubjectId.ToString(), model.ExamSourceId.ToString());
            return View(model);
        }

        TempData["ToastSuccess"] = "Questão criada com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var question = await client.GetFromJsonAsync<QuestionViewModel>(
            $"questions/{id}");

        if (question == null)
            return NotFound();

        var updateVm = MapToUpdateViewModel(question);

        await LoadSelectLists(question.SubjectId.ToString(), question.ExamSourceId.ToString());
        return View(updateVm);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Guid id, UpdateQuestionViewModel model)
    {
        if (!ModelState.IsValid) 
        {
            await LoadSelectLists();
            return View(model);
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        var response = await client.PutAsJsonAsync($"questions/{id}", model);

        if (!response.IsSuccessStatusCode)
        {
            await HandleApiFailure(response);
            await LoadSelectLists(model.SubjectId.ToString(), model.ExamSourceId.ToString());
            return View(model);
        }

        TempData["ToastSuccess"] = "Questão atualizada com sucesso!";
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

        var response = await client.DeleteAsync($"questions/{id}");

        if (response.IsSuccessStatusCode)
        {
            TempData["ToastSuccess"] = "Questão excluída com sucesso.";
        }
        else
        {
            TempData["ToastError"] = "Ocorreu um erro ao excluir a questão.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteBatch(List<Guid> ids)
    {
        if (ids == null || !ids.Any())
            return RedirectToAction(nameof(Index));

        var client = _httpClientFactory.CreateClient("ApiClient");
        bool hasError = false;

        foreach (var id in ids)
        {
            var response = await client.DeleteAsync($"questions/{id}");
            if (!response.IsSuccessStatusCode) hasError = true;
        }

        if (hasError) TempData["ToastWarning"] = "Algumas questões não puderam ser excluídas.";
        else TempData["ToastSuccess"] = "Questões selecionadas excluídas com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    private static UpdateQuestionViewModel MapToUpdateViewModel(QuestionViewModel question)
    {
        return new UpdateQuestionViewModel
        {
            Id = question.Id,
            Statement = question.Statement,
            Difficulty = question.Difficulty,
            SubjectId = question.SubjectId,
            ExamSourceId = question.ExamSourceId,
            ExamYear = question.ExamYear,
            Explanation = question.Explanation,
            AnswerOptions = question.AnswerOptions.Select(a => new UpdateAnswerOptionViewModel
            {
                Id = a.Id,
                Description = a.Description,
                IsCorrect = a.IsCorrect
            }).ToList()
        };
    }

    private async Task HandleApiFailure(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            try
            {
                // Tenta ler os erros de validação padrão do ASP.NET Core (ValidationProblemDetails)
                var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
                if (problemDetails?.Errors != null)
                {
                    foreach (var error in problemDetails.Errors)
                    {
                        foreach (var message in error.Value)
                        {
                            ModelState.AddModelError(error.Key, message);
                        }
                    }
                    return;
                }
            }
            catch { /* Ignora falha na deserialização e usa mensagem genérica */ }
        }

        ModelState.AddModelError("", "Ocorreu um erro ao processar a solicitação na API.");
    }

    private async Task LoadSelectLists(string? selectedSubjectId = null, string? selectedExamSourceId = null)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        try
        {
            // Busca as listas da API (assumindo endpoints 'subjects' e 'examsources')
            var subjects = await client.GetFromJsonAsync<List<NamedItem>>("subjects");
            ViewBag.Subjects = new SelectList(subjects ?? new(), "Id", "Name", selectedSubjectId);

            var sources = await client.GetFromJsonAsync<List<NamedItem>>("examsources");
            ViewBag.ExamSources = new SelectList(sources ?? new(), "Id", "Name", selectedExamSourceId);
        }
        catch (HttpRequestException) // Captura erros de conexão com a API
        {
            ViewBag.Subjects = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.ExamSources = new SelectList(Enumerable.Empty<SelectListItem>());
        }
    }

    private record NamedItem(Guid Id, string Name);
}
