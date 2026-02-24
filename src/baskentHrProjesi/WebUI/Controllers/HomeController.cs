using Application.Features.Dashboard.Dto;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Responses;
using System.Net.Http.Json;

[Route("[controller]/[action]")]
[SessionAuthorize]
public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetSummary()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.GetAsync("/api/Dashboard/Summary");
        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode);

        var summary = await resp.Content.ReadFromJsonAsync<GetDashboardSummaryResponse>();
        summary ??= new GetDashboardSummaryResponse();

        return Json(summary);
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployeeStatus()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.GetAsync("/api/Dashboard/EmployeeStatus");
        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode);

        var status = await resp.Content.ReadFromJsonAsync<GetDashboardEmployeeStatusResponse>();
        status ??= new GetDashboardEmployeeStatusResponse
        {
            PresentEmployees = new List<DashboardEmployeeDto>(),
            AbsentEmployees = new List<DashboardEmployeeDto>(),
            LeaveEmployees = new List<DashboardEmployeeDto>()
        };

        return Json(status);
    }

    [HttpGet]
    public async Task<IActionResult> GetUpcomingBirthdays()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.GetAsync("/api/Dashboard/UpcomingBirthdays");
        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode);

        var data = await resp.Content.ReadFromJsonAsync<GetDashboardUpcomingBirthdaysResponse>();
        data ??= new GetDashboardUpcomingBirthdaysResponse
        {
            UpcomingBirthdays = new List<DashboardEmployeeDto>()
        };

        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetLastEntries()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.GetAsync("/api/Dashboard/LastEntries");
        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode);

        var data = await resp.Content.ReadFromJsonAsync<GetDashboardLastEntriesResponse>();
        data ??= new GetDashboardLastEntriesResponse
        {
            LastEntries = new List<DashboardLastEntryDto>()
        };

        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetDepartmentCounts()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.GetAsync("/api/Dashboard/DepartmentCounts");
        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode);

        var data = await resp.Content.ReadFromJsonAsync<GetDashboardDepartmentCountsResponse>();
        data ??= new GetDashboardDepartmentCountsResponse
        {
            DepartmentCounts = new List<DashboardDepartmentCountDto>()
        };

        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetDepartmentDailyTrend()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.GetAsync("/api/Dashboard/DepartmentDailyTrend");
        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode);

        var data = await resp.Content.ReadFromJsonAsync<GetDashboardDepartmentDailyTrendResponse>();
        data ??= new GetDashboardDepartmentDailyTrendResponse
        {
            DepartmentDailyTrend = new List<DashboardDepartmentDailyTrendDto>()
        };

        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyMonthlySummary()
    {
        var client = _httpClientFactory.CreateClient("ApiClient");

        var resp = await client.GetAsync("/api/Dashboard/MyMonthlySummary");
        if (!resp.IsSuccessStatusCode)
            return StatusCode((int)resp.StatusCode);

        var data = await resp.Content.ReadFromJsonAsync<GetDashboardMyMonthlySummaryDto>();
        data ??= new GetDashboardMyMonthlySummaryDto();

        return Json(data);
    }

}
