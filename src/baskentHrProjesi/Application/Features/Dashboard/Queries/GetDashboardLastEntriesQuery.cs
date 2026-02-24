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

namespace Application.Features.Dashboard.Queries
{
    public class GetDashboardLastEntriesQuery : IRequest<GetDashboardLastEntriesResponse>, ISecuredRequest
    {
        public string[] Roles => [Admin, Read, "Personel"];

        public bool BypassCache => true;
        public string? CacheKey => null;
        public string? CacheGroupKey => null;
        public TimeSpan? SlidingExpiration => null;

        public class GetDashboardLastEntriesQueryHandler
            : IRequestHandler<GetDashboardLastEntriesQuery, GetDashboardLastEntriesResponse>
        {
            private readonly IEmployeeRepository _employeeRepository;
            private readonly ITimeEntryRepository _timeEntryRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public GetDashboardLastEntriesQueryHandler(
                IEmployeeRepository employeeRepository,
                ITimeEntryRepository timeEntryRepository,
                IHttpContextAccessor httpContextAccessor)
            {
                _employeeRepository = employeeRepository;
                _timeEntryRepository = timeEntryRepository;
                _httpContextAccessor = httpContextAccessor;
            }

            public async Task<GetDashboardLastEntriesResponse> Handle(
                GetDashboardLastEntriesQuery request,
                CancellationToken cancellationToken)
            {
                var today = DateTime.Today;

                var user = _httpContextAccessor.HttpContext?.User;

                int? currentEmployeeId = null;
                Employee? employee = null;

                var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userGuid))
                {
                    employee = await _employeeRepository.GetAsync(
                        e => e.UserId == userGuid,
                        enableTracking: false,
                        cancellationToken: cancellationToken
                    );

                    if (employee != null)
                    {
                        currentEmployeeId = employee.Id;
                    }
                }

                if (!currentEmployeeId.HasValue)
                {
                    return new GetDashboardLastEntriesResponse
                    {
                        LastEntries = new List<DashboardLastEntryDto>()
                    };
                }

                var monthStart = new DateTime(today.Year, today.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                IPaginate<TimeEntry> timeEntriesPage = await _timeEntryRepository.GetListAsync(
                    predicate: te =>
                        te.EmployeeId == currentEmployeeId.Value &&
                        te.EventTime >= monthStart &&
                        te.EventTime < monthEnd,
                    index: 0,
                    size: int.MaxValue,
                    cancellationToken: cancellationToken
                );

                var timeEntries = timeEntriesPage.Items?.ToList() ?? new List<TimeEntry>();

                var lastEntries = timeEntries
                    .OrderByDescending(te => te.EventTime)
                    .Select(te => new DashboardLastEntryDto
                    {
                        EmployeeId = te.EmployeeId,
                        EmployeeName = employee != null
                            ? ((employee.FirstName ?? string.Empty) + " " + (employee.LastName ?? string.Empty)).Trim()
                            : "ID " + te.EmployeeId,
                        EventTime = te.EventTime,
                        Direction = te.Direction
                    })
                    .ToList();

                return new GetDashboardLastEntriesResponse
                {
                    LastEntries = lastEntries
                };
            }
        }
    }
}
