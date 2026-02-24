using Application.Features.Timesheets.Calculate;
using Application.Features.Timesheets.Commands.Create;
using Application.Features.Timesheets.Commands.Update;
using Application.Features.Timesheets.Queries.GetById;
using Application.Features.Timesheets.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class TimesheetsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TimesheetsController(IHttpClientFactory httpClientFactory)
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

        var apiResponse = await client.GetFromJsonAsync<GetListResponse<GetListTimesheetListItemDto>>(
            "/api/Timesheets"
        );

        var items = apiResponse?.Items ?? new List<GetListTimesheetListItemDto>();

        return Json(new { data = items });
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var sheet = await client
            .GetFromJsonAsync<GetByIdTimesheetResponse>($"/api/Timesheets/{id}");

        if (sheet == null) return NotFound();

        return Json(sheet);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTimesheetCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/Timesheets", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Puantaj özeti eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateTimesheetCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/Timesheets", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Puantaj özeti güncellenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.DeleteAsync($"/api/Timesheets/{id}");

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Puantaj özeti silinemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> CalculateMonthly(CalculateMonthlyTimesheetQuery input)
    {
        if (input == null ||
            input.EmployeeIds == null ||
            !input.EmployeeIds.Any() ||
            input.Year <= 0 ||
            input.Month <= 0 || input.Month > 12)
        {
            return Json(new { success = false, message = "Geçersiz parametreler." });
        }

        var client = _httpClientFactory.CreateClient("ApiClient");

        var body = new
        {
            EmployeeIds = input.EmployeeIds,
            Year = input.Year,
            Month = input.Month
        };

        var resp = await client.PostAsJsonAsync("/api/Timesheets/CalculateMonthly", body);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Puantaj hesaplanamadı." });

        var monthly = await resp.Content
            .ReadFromJsonAsync<List<GetListTimesheetListItemDto>>();

        return Json(new { success = true, data = monthly });
    }
}
