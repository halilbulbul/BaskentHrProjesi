namespace Application.Features.Dashboard.Dto;

public class DashboardDepartmentDailyTrendDto
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Leave { get; set; }
    public int Late { get; set; }

}
