using Application.Features.LeaveTypes.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.LeaveTypes.Constants.LeaveTypesOperationClaims;

namespace Application.Features.LeaveTypes.Queries.GetList;

public class GetListLeaveTypeQuery : IRequest<GetListResponse<GetListLeaveTypeListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read, "Personel"];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListLeaveTypes({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetLeaveTypes";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListLeaveTypeQueryHandler : IRequestHandler<GetListLeaveTypeQuery, GetListResponse<GetListLeaveTypeListItemDto>>
    {
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IMapper _mapper;

        public GetListLeaveTypeQueryHandler(ILeaveTypeRepository leaveTypeRepository, IMapper mapper)
        {
            _leaveTypeRepository = leaveTypeRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListLeaveTypeListItemDto>> Handle(GetListLeaveTypeQuery request, CancellationToken cancellationToken)
        {
            IPaginate<LeaveType> leaveTypes = await _leaveTypeRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: int.MaxValue,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListLeaveTypeListItemDto> response = _mapper.Map<GetListResponse<GetListLeaveTypeListItemDto>>(leaveTypes);
            return response;
        }
    }
}