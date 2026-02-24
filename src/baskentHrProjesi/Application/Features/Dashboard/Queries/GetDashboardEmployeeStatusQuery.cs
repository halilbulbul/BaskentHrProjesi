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

public class GetDashboardEmployeeStatusQuery : IRequest<GetDashboardEmployeeStatusResponse>, ISecuredRequest
{
    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"DashboardEmployeeStatus({DateTime.Today:yyyyMMdd})";
    public string? CacheGroupKey => "Dashboard";
    public TimeSpan? SlidingExpiration { get; }

    public class GetDashboardEmployeeStatusQueryHandler
        : IRequestHandler<GetDashboardEmployeeStatusQuery, GetDashboardEmployeeStatusResponse>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly ITimeEntryRepository _timeEntryRepository;

        public GetDashboardEmployeeStatusQueryHandler(
            IEmployeeRepository employeeRepository,
            IEmployeeLeaveRepository employeeLeaveRepository,
            ITimeEntryRepository timeEntryRepository)
        {
            _employeeRepository = employeeRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _timeEntryRepository = timeEntryRepository;
        }

        public async Task<GetDashboardEmployeeStatusResponse> Handle(
            GetDashboardEmployeeStatusQuery request,
            CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            IPaginate<Employee> employeesPage = await _employeeRepository.GetListAsync(
                index: 0,
                size: int.MaxValue,
                cancellationToken: cancellationToken);

            IPaginate<EmployeeLeave> leavesPage = await _employeeLeaveRepository.GetListAsync(
                index: 0,
                size: int.MaxValue,
                cancellationToken: cancellationToken);

            IPaginate<TimeEntry> timeEntriesPage = await _timeEntryRepository.GetListAsync(
                predicate: te => te.EventTime >= today && te.EventTime < today.AddDays(1),
                index: 0,
                size: int.MaxValue,
                cancellationToken: cancellationToken);

            var employees = employeesPage.Items?.ToList() ?? new List<Employee>();
            var leaves = leavesPage.Items?.ToList() ?? new List<EmployeeLeave>();
            var timeEntries = timeEntriesPage.Items?.ToList() ?? new List<TimeEntry>();

            // Aktif personel = işe başlamış + ayrılmamış
            var activeEmployeesToday = employees
                .Where(e =>
                    e.HireDate.Date <= today &&
                    (!e.TerminationDate.HasValue || e.TerminationDate.Value.Date >= today))
                .ToList();

            var activeIds = activeEmployeesToday.Select(e => e.Id).ToHashSet();

            // Bugün giriş yapanlar
            var presentIds = timeEntries
                .Select(te => te.EmployeeId)
                .Distinct()
                .ToHashSet();
            presentIds.IntersectWith(activeIds);

            // Bugün onaylı izinde olanlar (null/empty => onaylı sayma)
            var leaveIds = leaves
                .Where(l =>
                    l.StartDate.Date <= today &&
                    l.EndDate.Date >= today &&
                    !string.IsNullOrEmpty(l.ApprovalStatus) &&
                    (l.ApprovalStatus.Equals("Onaylandı", StringComparison.OrdinalIgnoreCase) ||
                     l.ApprovalStatus.Equals("Approved", StringComparison.OrdinalIgnoreCase)))
                .Select(l => l.EmployeeId)
                .Distinct()
                .ToHashSet();
            leaveIds.IntersectWith(activeIds);

            // Çakışma çözümü: izinli olan present sayılmasın (tek kategori)
            presentIds.ExceptWith(leaveIds);

            // Geri kalanlar devamsız
            var absentIds = activeIds
                .Except(presentIds)
                .Except(leaveIds)
                .ToHashSet();

            var presentEmployeesToday = activeEmployeesToday.Where(e => presentIds.Contains(e.Id)).ToList();
            var leaveEmployeesToday = activeEmployeesToday.Where(e => leaveIds.Contains(e.Id)).ToList();
            var absentEmployeesToday = activeEmployeesToday.Where(e => absentIds.Contains(e.Id)).ToList();

            List<DashboardEmployeeDto> MapEmployees(List<Employee> list)
            {
                return list
                    .Select(e => new DashboardEmployeeDto
                    {
                        Id = e.Id,
                        FullName = ((e.FirstName ?? string.Empty) + " " + (e.LastName ?? string.Empty)).Trim()
                    })
                    .ToList();
            }

            return new GetDashboardEmployeeStatusResponse
            {
                PresentEmployees = MapEmployees(presentEmployeesToday),
                AbsentEmployees = MapEmployees(absentEmployeesToday),
                LeaveEmployees = MapEmployees(leaveEmployeesToday)
            };
        }
    }
}
