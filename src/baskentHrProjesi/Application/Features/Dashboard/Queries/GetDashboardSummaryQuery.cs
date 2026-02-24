using Application.Features.Dashboard.Dto;
using Application.Services.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
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

public class GetDashboardSummaryQuery : IRequest<GetDashboardSummaryResponse>, ISecuredRequest
{
    public string[] Roles => [Admin, Read, "Personel"];

    public bool BypassCache { get; }
    public string? CacheKey => $"DashboardSummary({DateTime.Today:yyyyMMdd})";
    public string? CacheGroupKey => "Dashboard";
    public TimeSpan? SlidingExpiration { get; }

    public class GetDashboardSummaryQueryHandler
        : IRequestHandler<GetDashboardSummaryQuery, GetDashboardSummaryResponse>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IShiftRepository _shiftRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetDashboardSummaryQueryHandler(
            IEmployeeRepository employeeRepository,
            IEmployeeLeaveRepository employeeLeaveRepository,
            ITimesheetRepository timesheetRepository,
            ITimeEntryRepository timeEntryRepository,
            IShiftRepository shiftRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _employeeRepository = employeeRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _timesheetRepository = timesheetRepository;
            _timeEntryRepository = timeEntryRepository;
            _shiftRepository = shiftRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetDashboardSummaryResponse> Handle(
            GetDashboardSummaryQuery request,
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

            IPaginate<Timesheet> timesheetsPage = await _timesheetRepository.GetListAsync(
                index: 0,
                size: int.MaxValue,
                cancellationToken: cancellationToken);

            var employees = employeesPage.Items?.ToList() ?? new List<Employee>();
            var leaves = leavesPage.Items?.ToList() ?? new List<EmployeeLeave>();
            var timesheets = timesheetsPage.Items?.ToList() ?? new List<Timesheet>();

            var activeEmployeesToday = employees
                .Where(e => !e.TerminationDate.HasValue || e.TerminationDate.Value.Date >= today)
                .ToList();

            var openLeavesToday = leaves
                .Where(l =>
                    l.StartDate.Date <= today &&
                    l.EndDate.Date >= today &&
                    (string.IsNullOrEmpty(l.ApprovalStatus) ||
                     l.ApprovalStatus.Equals("Beklemede", StringComparison.OrdinalIgnoreCase) ||
                     l.ApprovalStatus.Equals("Beklemede", StringComparison.OrdinalIgnoreCase)))
                .ToList();

            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            var newHires = activeEmployeesToday
                .Where(e => e.HireDate.Date >= firstDayOfMonth)
                .ToList();

            var currentMonthTimesheets = timesheets
                .Where(t => t.WorkDate.Year == today.Year && t.WorkDate.Month == today.Month)
                .ToList();

            var openDays = currentMonthTimesheets
                .Select(t => t.WorkDate.Day)
                .Distinct()
                .Count();

            // GEÇ GELENLER – GetDashboardLateEmployeesQuery mantığına göre
            IPaginate<TimeEntry> timeEntriesPage = await _timeEntryRepository.GetListAsync(
                predicate: te => te.EventTime >= today && te.EventTime < today.AddDays(1),
                index: 0,
                size: int.MaxValue,
                cancellationToken: cancellationToken);

            var timeEntries = timeEntriesPage.Items?.ToList() ?? new List<TimeEntry>();

            var shiftIds = activeEmployeesToday
                .Where(e => e.ShiftId.HasValue)
                .Select(e => e.ShiftId!.Value)
                .Distinct()
                .ToList();

            IPaginate<Shift> shiftsPage = await _shiftRepository.GetListAsync(
                predicate: s => shiftIds.Contains(s.Id),
                index: 0,
                size: int.MaxValue,
                cancellationToken: cancellationToken);

            var shifts = shiftsPage.Items?.ToList() ?? new List<Shift>();
            var shiftDict = shifts.ToDictionary(s => s.Id, s => s);

            var todayEntries = timeEntries
                .Where(te => te.EventTime.Date == today)
                .ToList();

            var lateEmployeeIds = new HashSet<int>();

            var entriesByEmployee = todayEntries
                .GroupBy(te => te.EmployeeId);

            foreach (var group in entriesByEmployee)
            {
                var emp = activeEmployeesToday.FirstOrDefault(e => e.Id == group.Key);
                if (emp == null)
                    continue;

                if (!emp.ShiftId.HasValue)
                    continue;

                if (!shiftDict.TryGetValue(emp.ShiftId.Value, out var shift))
                    continue;

                var lateThreshold = shift.ShiftStartTime.Add(TimeSpan.FromMinutes(shift.LateArrivalToleranceMinutes));

                var firstIn = group
                    .Where(te =>
                        string.Equals(te.Direction, "In", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(te.Direction, "Giriş", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(te => te.EventTime)
                    .FirstOrDefault();

                if (firstIn == null)
                    continue;

                if (firstIn.EventTime.TimeOfDay <= lateThreshold)
                    continue;

                lateEmployeeIds.Add(emp.Id);
            }

            var lateEmployeesTodayCount = lateEmployeeIds.Count;

            var user = _httpContextAccessor.HttpContext?.User;
            var isAdmin =
                user != null &&
                (user.IsInRole("Admin") || user.IsInRole("Dashboard.Admin"));

            return new GetDashboardSummaryResponse
            {
                ActiveEmployeeCount = activeEmployeesToday.Count,
                OpenLeaveCount = openLeavesToday.Count,
                NewHireCount = newHires.Count,
                OpenTimesheetDayCount = openDays,
                LateEmployeeCount = lateEmployeesTodayCount,
                IsAdmin = isAdmin
            };
        }
    }
}
