using Application.Features.Positions.Constants;
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

namespace Application.Features.Positions.Commands.Delete;

public class DeletePositionCommand : IRequest<DeletedPositionResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, PositionsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetPositions"];

    public class DeletePositionCommandHandler : IRequestHandler<DeletePositionCommand, DeletedPositionResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPositionRepository _positionRepository;
        private readonly PositionBusinessRules _positionBusinessRules;

        public DeletePositionCommandHandler(IMapper mapper, IPositionRepository positionRepository,
                                         PositionBusinessRules positionBusinessRules)
        {
            _mapper = mapper;
            _positionRepository = positionRepository;
            _positionBusinessRules = positionBusinessRules;
        }

        public async Task<DeletedPositionResponse> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
        {
            Position? position = await _positionRepository.GetAsync(predicate: p => p.Id == request.Id, cancellationToken: cancellationToken);
            await _positionBusinessRules.PositionShouldExistWhenSelected(position);

            await _positionRepository.DeleteAsync(position!);

            DeletedPositionResponse response = _mapper.Map<DeletedPositionResponse>(position);
            return response;
        }
    }
}