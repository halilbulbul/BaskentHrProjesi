using Application.Features.Positions.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.Positions.Constants.PositionsOperationClaims;

namespace Application.Features.Positions.Queries.GetList;

public class GetListPositionQuery : IRequest<GetListResponse<GetListPositionListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListPositions({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetPositions";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListPositionQueryHandler : IRequestHandler<GetListPositionQuery, GetListResponse<GetListPositionListItemDto>>
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IMapper _mapper;

        public GetListPositionQueryHandler(IPositionRepository positionRepository, IMapper mapper)
        {
            _positionRepository = positionRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListPositionListItemDto>> Handle(GetListPositionQuery request, CancellationToken cancellationToken)
        {
            IPaginate<Position> positions = await _positionRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: int.MaxValue, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListPositionListItemDto> response = _mapper.Map<GetListResponse<GetListPositionListItemDto>>(positions);
            return response;
        }
    }
}