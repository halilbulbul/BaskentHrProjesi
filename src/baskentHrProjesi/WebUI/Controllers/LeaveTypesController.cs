using Application.Features.EmployeeLeaves.Commands.Create;
using Application.Features.LeaveTypes.Commands.Create;
using Application.Features.LeaveTypes.Commands.Update;
using Application.Features.LeaveTypes.Queries.GetById;
using Application.Features.LeaveTypes.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class LeaveTypesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LeaveTypesController(IHttpClientFactory httpClientFactory)
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

        var apiResponse = await client.GetFromJsonAsync<GetListResponse<GetListLeaveTypeListItemDto>>("/api/LeaveTypes");

        var items = apiResponse?.Items ?? new List<GetListLeaveTypeListItemDto>();

        return Json(new { data = items });
    }

    [HttpGet]
    public async Task<IActionResult> Get(int id)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var leaveType = await client
            .GetFromJsonAsync<GetByIdLeaveTypeResponse>($"/api/LeaveTypes/{id}");

        if (leaveType == null) return NotFound();

        return Json(leaveType);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLeaveTypeCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Json(new { success = false, message = "İzin tipi adı zorunlu." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/LeaveTypes", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "İzin tipi eklenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Update(UpdateLeaveTypeCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PutAsJsonAsync("/api/LeaveTypes", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "İzin tipi güncellenemedi." });

        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        if (id <= 0)
            return Json(new { success = false, message = "Geçersiz id." });

        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.DeleteAsync($"/api/LeaveTypes/{id}");

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "İzin tipi silinemedi." });

        return Json(new { success = true });
    }

    [HttpGet]
    public async Task<IActionResult> EmployeeLeaveRequest()
    {
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> EmployeeLeaveRequest(CreateEmployeeLeaveCommand command)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.PostAsJsonAsync("/api/LeaveTypes/EmployeeLeaveRequest", command);

        if (!resp.IsSuccessStatusCode)
            return Json(new { success = false, message = "Personel izin talebi oluşturulamadı." });

        return Json(new { success = true });
    }
}
