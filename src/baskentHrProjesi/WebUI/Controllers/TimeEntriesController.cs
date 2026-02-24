using Application.Features.TimeEntries.Commands.Create;
using Application.Features.TimeEntries.Commands.Update;
using Application.Features.TimeEntries.Queries.GetById;
using Application.Features.TimeEntries.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class TimeEntriesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TimeEntriesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetList(int? employeeId, int? month, int? year)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var queryParts = new List<string>{};

        if (employeeId.HasValue)
            queryParts.Add($"employeeId={employeeId.Value}");

        if (year.HasValue)
            queryParts.Add($"year={year.Value}");

        if (month.HasValue)
            queryParts.Add($"month={month.Value}");

        var url = "/api/TimeEntries?" + string.Join("&", queryParts);

        var apiResponse =
            await client.GetFromJsonAsync<GetListResponse<GetListTimeEntryListItemDto>>(url);

        var items = apiResponse?.Items ?? new List<GetListTimeEntryListItemDto>();

        return Json(new { data = items });
    }



    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var entry = await client
            .GetFromJsonAsync<GetByIdTimeEntryResponse>($"/api/TimeEntries/{id}");

        if (entry == null) return NotFound();

        return Json(entry);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTimeEntryCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/TimeEntries", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Puantaj kaydı eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateTimeEntryCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/TimeEntries", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Puantaj kaydı güncellenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.DeleteAsync($"/api/TimeEntries/{id}");

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Puantaj kaydı silinemedi." });

        return Json(new { success = true });
    }
}
