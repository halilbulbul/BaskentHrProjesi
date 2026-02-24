using System;

namespace Application.Features.Dashboard.Dto;

public class DashboardLastEntryDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime EventTime { get; set; }
    public string Direction { get; set; }
}
