using Application.Features.Departments.Commands.Create;
using Application.Features.Departments.Commands.Update;
using Application.Features.Departments.Queries.GetById;
using Application.Features.Departments.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class DepartmentsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DepartmentsController(IHttpClientFactory httpClientFactory)
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

        var apiResponse = await client.GetFromJsonAsync<GetListResponse<GetListDepartmentListItemDto>>("/api/Departments");

        var items = apiResponse?.Items ?? new List<GetListDepartmentListItemDto>();

        return Json(new { data = items });
    }


    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var dept = await client
            .GetFromJsonAsync<GetByIdDepartmentResponse>($"/api/Departments/{id}");

        if (dept == null) return NotFound();

        return Json(dept);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Json(new { success = false, message = "Departman adı zorunlu." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/Departments", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Departman eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateDepartmentCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/Departments", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Departman güncellenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.DeleteAsync($"/api/Departments/{id}");

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Departman silinemedi." });

        return Json(new { success = true });
    }

}
