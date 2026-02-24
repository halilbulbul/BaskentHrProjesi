using Application.Features.Shifts.Constants;
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

namespace Application.Features.Shifts.Commands.Delete;

public class DeleteShiftCommand : IRequest<DeletedShiftResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, ShiftsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetShifts"];

    public class DeleteShiftCommandHandler : IRequestHandler<DeleteShiftCommand, DeletedShiftResponse>
    {
        private readonly IMapper _mapper;
        private readonly IShiftRepository _ShiftRepository;
        private readonly ShiftBusinessRules _ShiftBusinessRules;

        public DeleteShiftCommandHandler(IMapper mapper, IShiftRepository ShiftRepository,
                                         ShiftBusinessRules ShiftBusinessRules)
        {
            _mapper = mapper;
            _ShiftRepository = ShiftRepository;
            _ShiftBusinessRules = ShiftBusinessRules;
        }

        public async Task<DeletedShiftResponse> Handle(DeleteShiftCommand request, CancellationToken cancellationToken)
        {
            Shift? Shift = await _ShiftRepository.GetAsync(predicate: trs => trs.Id == request.Id, cancellationToken: cancellationToken);
            await _ShiftBusinessRules.ShiftshouldExistWhenSelected(Shift);

            await _ShiftRepository.DeleteAsync(Shift!);

            DeletedShiftResponse response = _mapper.Map<DeletedShiftResponse>(Shift);
            return response;
        }
    }
}