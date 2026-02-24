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

public class GetDashboardDailyTrendQuery : IRequest<GetDashboardDailyTrendResponse>, ISecuredRequest
{
    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"DashboardDailyTrend({DateTime.Today:yyyyMMdd})";
    public string? CacheGroupKey => "Dashboard";
    public TimeSpan? SlidingExpiration { get; }

    public class GetDashboardDailyTrendQueryHandler : IRequestHandler<GetDashboardDailyTrendQuery, GetDashboardDailyTrendResponse>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IShiftRepository _shiftRepository;

        public GetDashboardDailyTrendQueryHandler(
            IEmployeeRepository employeeRepository,
            IEmployeeLeaveRepository employeeLeaveRepository,
            ITimeEntryRepository timeEntryRepository,
            IShiftRepository shiftRepository)
        {
            _employeeRepository = employeeRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _timeEntryRepository = timeEntryRepository;
            _shiftRepository = shiftRepository;
        }

        public async Task<GetDashboardDailyTrendResponse> Handle(GetDashboardDailyTrendQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Employee> employeesPage = await _employeeRepository.GetListAsync(
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            IPaginate<EmployeeLeave> leavesPage = await _employeeLeaveRepository.GetListAsync(
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            IPaginate<TimeEntry> timeEntriesPage = await _timeEntryRepository.GetListAsync(
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            IPaginate<Shift> shiftsPage = await _shiftRepository.GetListAsync(
                index: 0, size: int.MaxValue, cancellationToken: cancellationToken);

            var defaultShift = shiftsPage.Items?.FirstOrDefault();
            TimeSpan? lateThresholdTime = null;

            if (defaultShift != null)
            {
                TimeSpan startTime = defaultShift.ShiftStartTime;
                int toleranceMinutes = defaultShift.LateArrivalToleranceMinutes;
                lateThresholdTime = startTime.Add(TimeSpan.FromMinutes(toleranceMinutes));
            }

            var employees = employeesPage.Items?.ToList() ?? new List<Employee>();
            var leaves = leavesPage.Items?.ToList() ?? new List<EmployeeLeave>();
            var timeEntries = timeEntriesPage.Items?.ToList() ?? new List<TimeEntry>();

            var dailyTrend = new List<DashboardDailyTrendItemDto>();

            var trendDates = timeEntries
                .Select(t => t.EventTime.Date)
                .Distinct()
                .OrderByDescending(d => d)
                .Take(7)
                .OrderBy(d => d)
                .ToList();

            foreach (var day in trendDates)
            {
                var activeOnDay = employees
                    .Where(e =>
                        e.HireDate.Date <= day &&
                        (!e.TerminationDate.HasValue || e.TerminationDate.Value.Date >= day))
                    .ToList();

                var entriesOnDay = timeEntries
                    .Where(te => te.EventTime.Date == day)
                    .ToList();

                var leavesOnDay = leaves
                    .Where(l => l.StartDate.Date <= day && l.EndDate.Date >= day)
                    .ToList();

                var presentIds = entriesOnDay
                    .Select(te => te.EmployeeId)
                    .Distinct()
                    .ToHashSet();

                var leaveIds = leavesOnDay
                    .Select(l => l.EmployeeId)
                    .Distinct()
                    .ToHashSet();

                var presentCount = activeOnDay.Count(e => presentIds.Contains(e.Id));
                var leaveCount = activeOnDay.Count(e => leaveIds.Contains(e.Id));
                var absentCount = activeOnDay.Count - presentCount - leaveCount;

                int lateCount = 0;

                if (lateThresholdTime.HasValue)
                {
                    var firstInByEmployee = entriesOnDay
                        .Where(te => string.Equals(te.Direction, "In", StringComparison.OrdinalIgnoreCase))
                        .GroupBy(te => te.EmployeeId)
                        .ToDictionary(
                            g => g.Key,
                            g => g.OrderBy(te => te.EventTime).First()
                        );

                    lateCount = activeOnDay.Count(e =>
                    {
                        if (!firstInByEmployee.TryGetValue(e.Id, out var firstIn))
                            return false;

                        if (leaveIds.Contains(e.Id))
                            return false;

                        var firstInTime = firstIn.EventTime.TimeOfDay;
                        return firstInTime > lateThresholdTime.Value;
                    });
                }

                dailyTrend.Add(new DashboardDailyTrendItemDto
                {
                    Date = day,
                    Present = presentCount,
                    Absent = absentCount,
                    Leave = leaveCount,
                    Late = lateCount
                });
            }

            return new GetDashboardDailyTrendResponse
            {
                DailyTrend = dailyTrend
            };
        }
    }
}
