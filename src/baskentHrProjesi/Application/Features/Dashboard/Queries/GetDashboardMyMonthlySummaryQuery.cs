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
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using static Application.Features.Dashboard.Constants.DashboardOperationClaims;

namespace Application.Features.Dashboard.Queries;

public class GetDashboardMyMonthlySummaryQuery
    : IRequest<GetDashboardMyMonthlySummaryDto>, ISecuredRequest
{
    public string[] Roles => [Admin, Read, "Personel"];

    public bool BypassCache => true;
    public string? CacheKey => null;
    public string? CacheGroupKey => null;
    public TimeSpan? SlidingExpiration => null;

    public class GetDashboardMyMonthlySummaryQueryHandler
        : IRequestHandler<GetDashboardMyMonthlySummaryQuery, GetDashboardMyMonthlySummaryDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetDashboardMyMonthlySummaryQueryHandler(
            IEmployeeRepository employeeRepository,
            ITimesheetRepository timesheetRepository,
            IEmployeeLeaveRepository employeeLeaveRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _employeeRepository = employeeRepository;
            _timesheet_repository = timesheetRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private readonly ITimesheetRepository _timesheet_repository;

        public async Task<GetDashboardMyMonthlySummaryDto> Handle(
            GetDashboardMyMonthlySummaryQuery request,
            CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            // Ay aralığı: [firstDay, firstDayNextMonth)
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
            var lastDayOfMonth = firstDayOfNextMonth.AddDays(-1);

            var user = _httpContextAccessor.HttpContext?.User;

            var employeeId = await ResolveEmployeeIdAsync(user, cancellationToken);
            if (!employeeId.HasValue)
            {
                return new GetDashboardMyMonthlySummaryDto
                {
                    OvertimeMinutes = 0,
                    LateArrivalMinutes = 0,
                    UsedLeaveDays = 0
                };
            }

            // ================== TIMESHEET (BU AY) ==================
            var timesheets = await GetAllTimesheetsForMonthAsync(
                employeeId.Value, firstDayOfMonth, firstDayOfNextMonth, cancellationToken);

            int overtimeMinutes = timesheets.Sum(t => t.OvertimeMinutes);
            int lateArrivalMinutes = timesheets.Sum(t => t.LateArrivalMinutes);

            // ================== LEAVES (BU AYLA KESİŞEN) ==================
            // EF translate problemi yaşamamak için ApprovalStatus filtresini DB içinde yapmıyoruz.
            var leavesRaw = await GetAllLeavesIntersectingMonthAsync(
                employeeId.Value, firstDayOfMonth, firstDayOfNextMonth, cancellationToken);

            // "Onaylandı" veya (istersen) boş/null olanları dahil et:
            // Not: senin eski mantığını bozmadım.
            var leaves = leavesRaw
                .Where(l =>
                    string.IsNullOrEmpty(l.ApprovalStatus) ||
                    l.ApprovalStatus.Trim().ToLower() == "onaylandı" ||
                    l.ApprovalStatus.Trim().ToLower() == "onaylandi")
                .ToList();

            int usedLeaveDays = 0;

            foreach (var leave in leaves)
            {
                var effectiveStart = leave.StartDate.Date < firstDayOfMonth
                    ? firstDayOfMonth
                    : leave.StartDate.Date;

                var effectiveEnd = leave.EndDate.Date > lastDayOfMonth
                    ? lastDayOfMonth
                    : leave.EndDate.Date;

                if (effectiveEnd >= effectiveStart)
                    usedLeaveDays += (int)(effectiveEnd - effectiveStart).TotalDays + 1;
            }

            return new GetDashboardMyMonthlySummaryDto
            {
                OvertimeMinutes = overtimeMinutes,
                LateArrivalMinutes = lateArrivalMinutes,
                UsedLeaveDays = usedLeaveDays
            };
        }

        private async Task<int?> ResolveEmployeeIdAsync(ClaimsPrincipal? user, CancellationToken cancellationToken)
        {
            if (user == null) return null;

            // 1) EmployeeId claim (en sağlamı)
            var empIdClaim = user.FindFirst("EmployeeId")?.Value ?? user.FindFirst("employeeId")?.Value;
            if (!string.IsNullOrWhiteSpace(empIdClaim) && int.TryParse(empIdClaim, out var empId))
            {
                var emp = await _employeeRepository.GetAsync(
                    e => e.Id == empId,
                    enableTracking: false,
                    cancellationToken: cancellationToken);

                if (emp != null) return emp.Id;
            }

            // 2) NameIdentifier / UserId / sub -> GUID ise Employee.UserId ile eşleştir
            var idValue =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                user.FindFirst("UserId")?.Value ??
                user.FindFirst("userId")?.Value ??
                user.FindFirst("sub")?.Value;

            if (!string.IsNullOrWhiteSpace(idValue) && Guid.TryParse(idValue, out var userGuid))
            {
                var emp = await _employeeRepository.GetAsync(
                    e => e.UserId == userGuid,
                    enableTracking: false,
                    cancellationToken: cancellationToken);

                if (emp != null) return emp.Id;
            }

            // 3) Email fallback (UserId boş kalan çalışanlar için)
            var email =
                user.FindFirst(ClaimTypes.Email)?.Value ??
                user.FindFirst("email")?.Value;

            if (!string.IsNullOrWhiteSpace(email))
            {
                email = email.Trim();

                var emp = await _employeeRepository.GetAsync(
                    e => e.Email == email,
                    enableTracking: false,
                    cancellationToken: cancellationToken);

                if (emp != null) return emp.Id;
            }

            return null;
        }

        private async Task<List<Timesheet>> GetAllTimesheetsForMonthAsync(
            int employeeId,
            DateTime monthStart,
            DateTime monthEndExclusive,
            CancellationToken cancellationToken)
        {
            var all = new List<Timesheet>();
            const int size = 500;
            int index = 0;

            while (true)
            {
                var page = await _timesheet_repository.GetListAsync(
                    predicate: t =>
                        t.EmployeeId == employeeId &&
                        t.WorkDate >= monthStart &&
                        t.WorkDate < monthEndExclusive,
                    index: index,
                    size: size,
                    withDeleted: false,
                    enableTracking: false,
                    cancellationToken: cancellationToken);

                if (page.Items != null && page.Items.Count > 0)
                    all.AddRange(page.Items);

                if (page.Items == null || page.Items.Count < size)
                    break;

                index++;
            }

            return all;
        }

        private async Task<List<EmployeeLeave>> GetAllLeavesIntersectingMonthAsync(
            int employeeId,
            DateTime monthStart,
            DateTime monthEndExclusive,
            CancellationToken cancellationToken)
        {
            var all = new List<EmployeeLeave>();
            const int size = int.MaxValue;
            int index = 0;

            while (true)
            {
                var page = await _employeeLeaveRepository.GetListAsync(
                    predicate: l =>
                        l.EmployeeId == employeeId &&
                        l.EndDate >= monthStart &&
                        l.StartDate < monthEndExclusive,
                    index: index,
                    size: size,
                    withDeleted: false,
                    enableTracking: false,
                    cancellationToken: cancellationToken);

                if (page.Items != null && page.Items.Count > 0)
                    all.AddRange(page.Items);

                if (page.Items == null || page.Items.Count < size)
                    break;

                index++;
            }

            return all;
        }
    }
}
