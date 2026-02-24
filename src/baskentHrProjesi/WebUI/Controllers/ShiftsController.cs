using Application.Features.Shifts.Commands.Create;
using Application.Features.Shifts.Commands.Update;
using Application.Features.Shifts.Queries.GetById;
using Application.Features.Shifts.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class ShiftsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ShiftsController(IHttpClientFactory httpClientFactory)
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

        var apiResponse = await client.GetFromJsonAsync<GetListResponse<GetListShiftListItemDto>>(
            "/api/Shifts"
        );

        var items = apiResponse?.Items ?? new List<GetListShiftListItemDto>();

        return Json(new { data = items });
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var ruleset = await client
            .GetFromJsonAsync<GetByIdShiftResponse>($"/api/Shifts/{id}");

        if (ruleset == null) return NotFound();

        return Json(ruleset);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateShiftCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/Shifts", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Vardiya kural seti eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateShiftCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/Shifts", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Vardiya kural seti güncellenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.DeleteAsync($"/api/Shifts/{id}");

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Vardiya kural seti silinemedi." });

        return Json(new { success = true });
    }
}
