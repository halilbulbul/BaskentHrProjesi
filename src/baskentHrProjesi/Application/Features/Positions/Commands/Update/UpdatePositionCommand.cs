using Application.Features.Positions.Constants;
using Application.Features.Positions.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Positions.Constants.PositionsOperationClaims;

namespace Application.Features.Positions.Commands.Update;

public class UpdatePositionCommand : IRequest<UpdatedPositionResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }

    public string[] Roles => [Admin, Write, PositionsOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetPositions"];

    public class UpdatePositionCommandHandler : IRequestHandler<UpdatePositionCommand, UpdatedPositionResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPositionRepository _positionRepository;
        private readonly PositionBusinessRules _positionBusinessRules;

        public UpdatePositionCommandHandler(IMapper mapper, IPositionRepository positionRepository,
                                         PositionBusinessRules positionBusinessRules)
        {
            _mapper = mapper;
            _positionRepository = positionRepository;
            _positionBusinessRules = positionBusinessRules;
        }

        public async Task<UpdatedPositionResponse> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
        {
            Position? position = await _positionRepository.GetAsync(predicate: p => p.Id == request.Id, cancellationToken: cancellationToken);
            await _positionBusinessRules.PositionShouldExistWhenSelected(position);
            position = _mapper.Map(request, position);

            await _positionRepository.UpdateAsync(position!);

            UpdatedPositionResponse response = _mapper.Map<UpdatedPositionResponse>(position);
            return response;
        }
    }
}