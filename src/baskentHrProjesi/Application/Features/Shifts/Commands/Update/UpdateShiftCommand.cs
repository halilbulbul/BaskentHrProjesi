using Application.Features.Shifts.Constants;
using Application.Features.Shifts.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Shifts.Constants.ShiftsOperationClaims;

namespace Application.Features.Shifts.Commands.Update;

public class UpdateShiftCommand : IRequest<UpdatedShiftResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required TimeSpan ShiftStartTime { get; set; }
    public required TimeSpan ShiftEndTime { get; set; }
    public TimeSpan? BreakStartTime { get; set; }
    public TimeSpan? BreakEndTime { get; set; }
    public required int EarlyArrivalToleranceMinutes { get; set; }
    public required int LateArrivalToleranceMinutes { get; set; }
    public required int EarlyLeaveToleranceMinutes { get; set; }
    public required int LateLeaveToleranceMinutes { get; set; }

    public string[] Roles => [Admin, Write, ShiftsOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetShifts"];

    public class UpdateShiftCommandHandler : IRequestHandler<UpdateShiftCommand, UpdatedShiftResponse>
    {
        private readonly IMapper _mapper;
        private readonly IShiftRepository _ShiftRepository;
        private readonly ShiftBusinessRules _ShiftBusinessRules;

        public UpdateShiftCommandHandler(IMapper mapper, IShiftRepository ShiftRepository,
                                         ShiftBusinessRules ShiftBusinessRules)
        {
            _mapper = mapper;
            _ShiftRepository = ShiftRepository;
            _ShiftBusinessRules = ShiftBusinessRules;
        }

        public async Task<UpdatedShiftResponse> Handle(UpdateShiftCommand request, CancellationToken cancellationToken)
        {
            Shift? Shift = await _ShiftRepository.GetAsync(predicate: trs => trs.Id == request.Id, cancellationToken: cancellationToken);
            await _ShiftBusinessRules.ShiftshouldExistWhenSelected(Shift);
            Shift = _mapper.Map(request, Shift);

            await _ShiftRepository.UpdateAsync(Shift!);

            UpdatedShiftResponse response = _mapper.Map<UpdatedShiftResponse>(Shift);
            return response;
        }
    }
}