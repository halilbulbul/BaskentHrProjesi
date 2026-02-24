using System;

namespace Application.Features.Dashboard.Dto;

public class DashboardDailyTrendItemDto
{
    public DateTime Date { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Leave { get; set; }
    public int Late { get; set; }

}
