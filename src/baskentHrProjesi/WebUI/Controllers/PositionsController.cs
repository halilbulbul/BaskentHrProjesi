using Application.Features.Positions.Commands.Create;
using Application.Features.Positions.Commands.Update;
using Application.Features.Positions.Queries.GetById;
using Application.Features.Positions.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class PositionsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public PositionsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var apiResponse = await client.GetFromJsonAsync<GetListResponse<GetListPositionListItemDto>>("/api/Positions");

        var items = apiResponse?.Items ?? new List<GetListPositionListItemDto>();

        return Json(new { data = items });
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var pos = await client
            .GetFromJsonAsync<GetByIdPositionResponse>($"/api/Positions/{id}");

        if (pos == null) return NotFound();

        return Json(pos);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePositionCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Json(new { success = false, message = "Pozisyon adı zorunlu." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/Positions", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Pozisyon eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdatePositionCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/Positions", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Pozisyon güncellenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.DeleteAsync($"/api/Positions/{id}");

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Pozisyon silinemedi." });

        return Json(new { success = true });
    }
}
