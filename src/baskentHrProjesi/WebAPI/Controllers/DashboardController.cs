using Application.Features.Dashboard.Dto;
using Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : BaseController
{
    [HttpGet("Summary")]
    public async Task<ActionResult<GetDashboardSummaryResponse>> GetSummary()
    {
        GetDashboardSummaryQuery query = new();
        GetDashboardSummaryResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("EmployeeStatus")]
    public async Task<ActionResult<GetDashboardEmployeeStatusResponse>> GetEmployeeStatus()
    {
        GetDashboardEmployeeStatusQuery query = new();
        GetDashboardEmployeeStatusResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("UpcomingBirthdays")]
    public async Task<ActionResult<GetDashboardUpcomingBirthdaysResponse>> GetUpcomingBirthdays()
    {
        GetDashboardUpcomingBirthdaysQuery query = new();
        GetDashboardUpcomingBirthdaysResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("LastEntries")]
    public async Task<ActionResult<GetDashboardLastEntriesResponse>> GetLastEntries()
    {
        GetDashboardLastEntriesQuery query = new();
        GetDashboardLastEntriesResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("DepartmentCounts")]
    public async Task<ActionResult<GetDashboardDepartmentCountsResponse>> GetDepartmentCounts()
    {
        GetDashboardDepartmentCountsQuery query = new();
        GetDashboardDepartmentCountsResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("DailyTrend")]
    public async Task<ActionResult<GetDashboardDailyTrendResponse>> GetDailyTrend()
    {
        GetDashboardDailyTrendQuery query = new();
        GetDashboardDailyTrendResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("DepartmentDailyTrend")]
    public async Task<ActionResult<GetDashboardDepartmentDailyTrendResponse>> GetDepartmentDailyTrend()
    {
        GetDashboardDepartmentDailyTrendQuery query = new();
        GetDashboardDepartmentDailyTrendResponse response = await Mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("MyMonthlySummary")]
    public async Task<ActionResult<GetDashboardMyMonthlySummaryDto>> GetMyMonthlySummary()
    {
        GetDashboardMyMonthlySummaryQuery query = new();
        GetDashboardMyMonthlySummaryDto response = await Mediator.Send(query);
        return Ok(response);
    }

}
