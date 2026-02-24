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

public class GetDashboardUpcomingBirthdaysQuery : IRequest<GetDashboardUpcomingBirthdaysResponse>, ISecuredRequest
{
    public string[] Roles => [Admin, Read, "Personel"];

    public bool BypassCache { get; }
    public string? CacheKey => $"DashboardUpcomingBirthdays({DateTime.Today:yyyyMM})";
    public string? CacheGroupKey => "Dashboard";
    public TimeSpan? SlidingExpiration { get; }

    public class GetDashboardUpcomingBirthdaysQueryHandler
        : IRequestHandler<GetDashboardUpcomingBirthdaysQuery, GetDashboardUpcomingBirthdaysResponse>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetDashboardUpcomingBirthdaysQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<GetDashboardUpcomingBirthdaysResponse> Handle(
            GetDashboardUpcomingBirthdaysQuery request,
            CancellationToken cancellationToken)
        {
            var today = DateTime.Today;
            var currentMonth = today.Month;

            IPaginate<Employee> employeesPage = await _employeeRepository.GetListAsync(
                index: 0,
                size: int.MaxValue,
                cancellationToken: cancellationToken
            );

            var employees = employeesPage.Items?.ToList() ?? new List<Employee>();

            var activeEmployeesToday = employees
                .Where(e => !e.TerminationDate.HasValue || e.TerminationDate.Value.Date >= today)
                .ToList();

            var upcomingBirthdays = activeEmployeesToday
                .Where(e =>
                    e.BirthDate.HasValue &&
                    e.BirthDate.Value.Month == currentMonth)
                .OrderBy(e => e.BirthDate!.Value.Day)
                .ToList();

            var list = upcomingBirthdays
                .Select(e => new DashboardEmployeeDto
                {
                    Id = e.Id,
                    FullName = ((e.FirstName ?? string.Empty) + " " + (e.LastName ?? string.Empty)).Trim()
                })
                .ToList();

            return new GetDashboardUpcomingBirthdaysResponse
            {
                UpcomingBirthdays = list
            };
        }
    }
}
