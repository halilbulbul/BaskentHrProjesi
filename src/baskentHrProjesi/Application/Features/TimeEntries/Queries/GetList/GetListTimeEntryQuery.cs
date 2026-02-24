using Application.Features.TimeEntries.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Dynamic;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.TimeEntries.Constants.TimeEntriesOperationClaims;

namespace Application.Features.TimeEntries.Queries.GetList;

public class GetListTimeEntryQuery : IRequest<GetListResponse<GetListTimeEntryListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; } = null!;

    // FİLTRELER
    public int? EmployeeId { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }

    // Dynamic sorgu
    public DynamicQuery Dynamic { get; set; } = null!;

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey =>
        $"GetListTimeEntries({PageRequest.PageIndex},{PageRequest.PageSize},{EmployeeId},{Year},{Month})";
    public string? CacheGroupKey => "GetTimeEntries";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListTimeEntryQueryHandler
        : IRequestHandler<GetListTimeEntryQuery, GetListResponse<GetListTimeEntryListItemDto>>
    {
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IMapper _mapper;

        public GetListTimeEntryQueryHandler(ITimeEntryRepository timeEntryRepository, IMapper mapper)
        {
            _timeEntryRepository = timeEntryRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListTimeEntryListItemDto>> Handle(
            GetListTimeEntryQuery request,
            CancellationToken cancellationToken)
        {
            IPaginate<TimeEntry> timeEntries = await _timeEntryRepository.GetListByDynamicAsync(
                include: q => q.Include(t => t.Employee),
                dynamic: request.Dynamic,
                predicate: te =>
                    (!request.EmployeeId.HasValue || te.EmployeeId == request.EmployeeId.Value) &&
                    (!request.Year.HasValue || te.EventTime.Year == request.Year.Value) &&
                    (!request.Month.HasValue || te.EventTime.Month == request.Month.Value),
                index: request.PageRequest.PageIndex,
                size: int.MaxValue,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListTimeEntryListItemDto> response =
                _mapper.Map<GetListResponse<GetListTimeEntryListItemDto>>(timeEntries);

            return response;
        }
    }
}
