using Application.Features.Timesheets.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Timesheets.Constants.TimesheetsOperationClaims;

namespace Application.Features.Timesheets.Queries.GetList;

public class GetListTimesheetQuery : IRequest<GetListResponse<GetListTimesheetListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListTimesheets({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetTimesheets";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListTimesheetQueryHandler : IRequestHandler<GetListTimesheetQuery, GetListResponse<GetListTimesheetListItemDto>>
    {
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly IMapper _mapper;

        public GetListTimesheetQueryHandler(ITimesheetRepository timesheetRepository, IMapper mapper)
        {
            _timesheetRepository = timesheetRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListTimesheetListItemDto>> Handle(GetListTimesheetQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Timesheet> timesheets = await _timesheetRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: int.MaxValue, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListTimesheetListItemDto> response = _mapper.Map<GetListResponse<GetListTimesheetListItemDto>>(timesheets);
            return response;
        }
    }
}