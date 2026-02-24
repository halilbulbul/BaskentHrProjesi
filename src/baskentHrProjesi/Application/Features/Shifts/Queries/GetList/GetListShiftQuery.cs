using Application.Features.Shifts.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Shifts.Constants.ShiftsOperationClaims;

namespace Application.Features.Shifts.Queries.GetList;

public class GetListShiftQuery : IRequest<GetListResponse<GetListShiftListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListShifts({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetShifts";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListShiftQueryHandler : IRequestHandler<GetListShiftQuery, GetListResponse<GetListShiftListItemDto>>
    {
        private readonly IShiftRepository _ShiftRepository;
        private readonly IMapper _mapper;

        public GetListShiftQueryHandler(IShiftRepository ShiftRepository, IMapper mapper)
        {
            _ShiftRepository = ShiftRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListShiftListItemDto>> Handle(GetListShiftQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Shift> Shifts = await _ShiftRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: int.MaxValue, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListShiftListItemDto> response = _mapper.Map<GetListResponse<GetListShiftListItemDto>>(Shifts);
            return response;
        }
    }
}