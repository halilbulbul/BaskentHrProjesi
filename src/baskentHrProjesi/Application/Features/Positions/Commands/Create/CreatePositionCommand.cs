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

namespace Application.Features.Positions.Commands.Create;

public class CreatePositionCommand : IRequest<CreatedPositionResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required string Code { get; set; }
    public required string Name { get; set; }

    public string[] Roles => [Admin, Write, PositionsOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetPositions"];

    public class CreatePositionCommandHandler : IRequestHandler<CreatePositionCommand, CreatedPositionResponse>
    {
        private readonly IMapper _mapper;
        private readonly IPositionRepository _positionRepository;
        private readonly PositionBusinessRules _positionBusinessRules;

        public CreatePositionCommandHandler(IMapper mapper, IPositionRepository positionRepository,
                                         PositionBusinessRules positionBusinessRules)
        {
            _mapper = mapper;
            _positionRepository = positionRepository;
            _positionBusinessRules = positionBusinessRules;
        }

        public async Task<CreatedPositionResponse> Handle(CreatePositionCommand request, CancellationToken cancellationToken)
        {
            Position position = _mapper.Map<Position>(request);

            await _positionRepository.AddAsync(position);

            CreatedPositionResponse response = _mapper.Map<CreatedPositionResponse>(position);
            return response;
        }
    }
}