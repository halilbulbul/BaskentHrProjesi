using Application.Features.Positions.Constants;
using Application.Features.Positions.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Positions.Constants.PositionsOperationClaims;

namespace Application.Features.Positions.Queries.GetById;

public class GetByIdPositionQuery : IRequest<GetByIdPositionResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdPositionQueryHandler : IRequestHandler<GetByIdPositionQuery, GetByIdPositionResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPositionRepository _positionRepository;
        private readonly PositionBusinessRules _positionBusinessRules;

        public GetByIdPositionQueryHandler(IMapper mapper, IPositionRepository positionRepository, PositionBusinessRules positionBusinessRules)
        {
            _mapper = mapper;
            _positionRepository = positionRepository;
            _positionBusinessRules = positionBusinessRules;
        }

        public async Task<GetByIdPositionResponse> Handle(GetByIdPositionQuery request, CancellationToken cancellationToken)
        {
            Position? position = await _positionRepository.GetAsync(predicate: p => p.Id == request.Id, cancellationToken: cancellationToken);
            await _positionBusinessRules.PositionShouldExistWhenSelected(position);

            GetByIdPositionResponse response = _mapper.Map<GetByIdPositionResponse>(position);
            return response;
        }
    }
}