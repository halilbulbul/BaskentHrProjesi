using Application.Features.Dashboard.Queries;
using Application.Features.Employees.Commands.Create;
using Application.Features.Employees.Commands.Update;
using Application.Features.Employees.Queries.GetById;
using Application.Features.Employees.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class EmployeesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EmployeesController(IHttpClientFactory httpClientFactory)
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

        var apiResponse = await client.GetFromJsonAsync<GetListResponse<GetListEmployeeListItemDto>>("/api/Employees");

        var items = apiResponse?.Items ?? new List<GetListEmployeeListItemDto>();

        return Json(new { data = items });
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var emp = await client
            .GetFromJsonAsync<GetByIdEmployeeResponse>($"/api/Employees/{id}");

        if (emp == null) return NotFound();

        return Json(emp);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/Employees", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Personel eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateEmployeeCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/Employees", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Personel güncellenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.DeleteAsync($"/api/Employees/{id}");

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Personel silinemedi." });

        return Json(new { success = true });
    }
}
