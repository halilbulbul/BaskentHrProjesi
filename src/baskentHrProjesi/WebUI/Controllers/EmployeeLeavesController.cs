using Application.Features.EmployeeLeaves.Commands.Create;
using Application.Features.EmployeeLeaves.Commands.Update;
using Application.Features.EmployeeLeaves.Queries.GetById;
using Application.Features.EmployeeLeaves.Queries.GetList;
using Application.Features.EmployeeLeaves.Queries.GetListByCurrentUser;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class EmployeeLeavesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EmployeeLeavesController(IHttpClientFactory httpClientFactory)
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

        var apiResponse = await client.GetFromJsonAsync<GetListResponse<GetListEmployeeLeaveListItemDto>>("/api/EmployeeLeaves");

        var items = apiResponse?.Items ?? new List<GetListEmployeeLeaveListItemDto>();

        return Json(new { data = items });
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var leave = await client
            .GetFromJsonAsync<GetByIdEmployeeLeaveResponse>($"/api/EmployeeLeaves/{id}");

        if (leave == null) return NotFound();

        return Json(leave);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateEmployeeLeaveCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/EmployeeLeaves", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Personel izni eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateEmployeeLeaveCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/EmployeeLeaves", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Personel izni güncellenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.DeleteAsync($"/api/EmployeeLeaves/{id}");

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Personel izni silinemedi." });

        return Json(new { success = true });
    }

    [HttpGet]
    public IActionResult GetLeaves()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetMyList()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var apiResponse =
            await client.GetFromJsonAsync<GetListResponse<GetListByUserEmployeeLeaveListItemDto>>("/api/EmployeeLeaves/GetMyLeaves");

        var items = apiResponse?.Items ?? new List<GetListByUserEmployeeLeaveListItemDto>();

        return Json(new { data = items });
    }

    [HttpPost]
    public async Task<IActionResult> EmployeeLeaveRequest(CreateEmployeeLeaveCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/EmployeeLeaves/EmployeeLeaveRequest", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "İzin talebi oluşturulamadı." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Approve(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync($"/api/EmployeeLeaves/{id}/approve", new { });

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "İzin talebi onaylanamadı." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Reject(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync($"/api/EmployeeLeaves/{id}/reject", new { });

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "İzin talebi reddedilemedi." });

        return Json(new { success = true });
    }


}
