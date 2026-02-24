using Application.Features.Dashboard.Dto;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Persistence.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Application.Features.Dashboard.Constants.DashboardOperationClaims;

namespace Application.Features.Dashboard.Queries;

public class GetDashboardDepartmentCountsQuery : IRequest<GetDashboardDepartmentCountsResponse>, ISecuredRequest
{
    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"DashboardDepartmentCounts({DateTime.Today:yyyyMMdd})";
    public string? CacheGroupKey => "Dashboard";
    public TimeSpan? SlidingExpiration { get; }

    public class GetDashboardDepartmentCountsQueryHandler : IRequestHandler<GetDashboardDepartmentCountsQuery, GetDashboardDepartmentCountsResponse>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        public GetDashboardDepartmentCountsQueryHandler(
            IEmployeeRepository employeeRepository,
            IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<GetDashboardDepartmentCountsResponse> Handle(GetDashboardDepartmentCountsQuery request, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            IPaginate<Employee> employeesPage = await _employeeRepository.GetListAsync(
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            IPaginate<Department> departmentsPage = await _departmentRepository.GetListAsync(
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            var employees = employeesPage.Items?.ToList() ?? new List<Employee>();
            var departments = departmentsPage.Items?.ToList() ?? new List<Department>();

            var activeEmployeesToday = employees
                .Where(e => !e.TerminationDate.HasValue || e.TerminationDate.Value.Date >= today)
                .ToList();

            var deptCounts = activeEmployeesToday
                .GroupBy(e => e.DepartmentId)
                .Join(
                    departments,
                    g => g.Key,
                    d => d.Id,
                    (g, d) => new DashboardDepartmentCountDto
                    {
                        Id = d.Id,
                        Name = d.Name ?? string.Empty,
                        Count = g.Count()
                    })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            return new GetDashboardDepartmentCountsResponse
            {
                DepartmentCounts = deptCounts
            };
        }
    }
}
