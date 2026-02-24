// GetDashboardDepartmentDailyTrendQuery.cs
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

public class GetDashboardDepartmentDailyTrendQuery
    : IRequest<GetDashboardDepartmentDailyTrendResponse>, ISecuredRequest
{
    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"DashboardDepartmentDailyTrend({DateTime.Today:yyyyMMdd})";
    public string? CacheGroupKey => "Dashboard";
    public TimeSpan? SlidingExpiration { get; }

    public class GetDashboardDepartmentDailyTrendQueryHandler
        : IRequestHandler<GetDashboardDepartmentDailyTrendQuery, GetDashboardDepartmentDailyTrendResponse>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IShiftRepository _shiftRepository;

        public GetDashboardDepartmentDailyTrendQueryHandler(
            IEmployeeRepository employeeRepository,
            IEmployeeLeaveRepository employeeLeaveRepository,
            ITimeEntryRepository timeEntryRepository,
            IDepartmentRepository departmentRepository,
            IShiftRepository shiftRepository)
        {
            _employeeRepository = employeeRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _timeEntryRepository = timeEntryRepository;
            _departmentRepository = departmentRepository;
            _shiftRepository = shiftRepository;
        }

        public async Task<GetDashboardDepartmentDailyTrendResponse> Handle(
            GetDashboardDepartmentDailyTrendQuery request,
            CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            IPaginate<Employee> employeesPage = await _employeeRepository.GetListAsync(
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            IPaginate<EmployeeLeave> leavesPage = await _employeeLeaveRepository.GetListAsync(
                // sadece bugünü kapsayanlar
                predicate: l => l.StartDate <= tomorrow && l.EndDate >= today,
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            IPaginate<TimeEntry> timeEntriesPage = await _timeEntryRepository.GetListAsync(
                // sadece bugün
                predicate: te => te.EventTime >= today && te.EventTime < tomorrow,
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            IPaginate<Department> departmentsPage = await _departmentRepository.GetListAsync(
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            IPaginate<Shift> shiftsPage = await _shiftRepository.GetListAsync(
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            var employees = employeesPage.Items?.ToList() ?? new List<Employee>();
            var leaves = leavesPage.Items?.ToList() ?? new List<EmployeeLeave>();
            var todayEntries = timeEntriesPage.Items?.ToList() ?? new List<TimeEntry>();
            var departments = departmentsPage.Items?.ToList() ?? new List<Department>();
            var shifts = shiftsPage.Items?.ToList() ?? new List<Shift>();

            // aktif + işe başlamış olmalı
            var activeEmployeesToday = employees
                .Where(e =>
                    e.HireDate.Date <= today &&
                    (!e.TerminationDate.HasValue || e.TerminationDate.Value.Date >= today))
                .ToList();

            // bugünkü herhangi bir kayıt = "burada" sayılır (ama kategorilerde ayrıca ele alacağız)
            var presentEmployeeIdsToday = todayEntries
                .Select(te => te.EmployeeId)
                .Distinct()
                .ToHashSet();

            // izinli (onaylı veya boş) -> gün bazında geçerli
            var effectiveLeavesToday = leaves
                .Where(l =>
                    l.StartDate.Date <= today &&
                    l.EndDate.Date >= today &&
                    (string.IsNullOrEmpty(l.ApprovalStatus) ||
                     l.ApprovalStatus.Equals("Onaylandı", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var leaveEmployeeIdsToday = effectiveLeavesToday
                .Select(l => l.EmployeeId)
                .Distinct()
                .ToHashSet();

            // geç kalma eşiği (varsayılan shift)
            var defaultShift = shifts.FirstOrDefault();
            TimeSpan? lateThresholdTime = null;

            if (defaultShift != null)
            {
                lateThresholdTime =
                    defaultShift.ShiftStartTime.Add(TimeSpan.FromMinutes(defaultShift.LateArrivalToleranceMinutes));
            }

            // geç gelenler (izinliler hariç)
            var lateEmployeeIdsToday = new HashSet<int>();

            if (lateThresholdTime.HasValue)
            {
                var firstInByEmployeeToday = todayEntries
                    .Where(te => string.Equals(te.Direction, "In", StringComparison.OrdinalIgnoreCase))
                    .GroupBy(te => te.EmployeeId)
                    .ToDictionary(g => g.Key, g => g.OrderBy(te => te.EventTime).First());

                foreach (var e in activeEmployeesToday)
                {
                    if (leaveEmployeeIdsToday.Contains(e.Id))
                        continue;

                    if (!firstInByEmployeeToday.TryGetValue(e.Id, out var firstIn))
                        continue;

                    if (firstIn.EventTime.TimeOfDay > lateThresholdTime.Value)
                        lateEmployeeIdsToday.Add(e.Id);
                }
            }

            var departmentsById = departments.ToDictionary(d => d.Id);

            // KATEGORİLER MUTLAKA AYRIŞIK:
            // Leave, Late, Present(=zamanında), Absent
            var departmentDailyTrend = activeEmployeesToday
                .Where(e => e.DepartmentId != 0)
                .GroupBy(e => e.DepartmentId)
                .Select(g =>
                {
                    departmentsById.TryGetValue(g.Key, out var dept);
                    var empIds = g.Select(e => e.Id).ToList();

                    var leaveCount = empIds.Count(id => leaveEmployeeIdsToday.Contains(id));
                    var lateCount = empIds.Count(id => lateEmployeeIdsToday.Contains(id));

                    // Present = giriş yapmış, izinli değil, geç değil
                    var presentCount = empIds.Count(id =>
                        presentEmployeeIdsToday.Contains(id) &&
                        !leaveEmployeeIdsToday.Contains(id) &&
                        !lateEmployeeIdsToday.Contains(id));

                    var absentCount = empIds.Count - (presentCount + lateCount + leaveCount);

                    return new DashboardDepartmentDailyTrendDto
                    {
                        DepartmentId = g.Key,
                        DepartmentName = dept?.Name ?? "Tanımsız",
                        Present = presentCount,
                        Late = lateCount,
                        Leave = leaveCount,
                        Absent = absentCount
                    };
                })
                .OrderBy(x => x.DepartmentName)
                .ToList();

            return new GetDashboardDepartmentDailyTrendResponse
            {
                DepartmentDailyTrend = departmentDailyTrend
            };
        }
    }
}
