using Application.Features.LeaveTypes.Constants;
using Application.Features.LeaveTypes.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.LeaveTypes.Constants.LeaveTypesOperationClaims;

namespace Application.Features.LeaveTypes.Commands.Update;

public class UpdateLeaveTypeCommand : IRequest<UpdatedLeaveTypeResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required bool IsPaid { get; set; }

    public string[] Roles => [Admin, Write, LeaveTypesOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetLeaveTypes"];

    public class UpdateLeaveTypeCommandHandler : IRequestHandler<UpdateLeaveTypeCommand, UpdatedLeaveTypeResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly LeaveTypeBusinessRules _leaveTypeBusinessRules;

        public UpdateLeaveTypeCommandHandler(IMapper mapper, ILeaveTypeRepository leaveTypeRepository,
                                         LeaveTypeBusinessRules leaveTypeBusinessRules)
        {
            _mapper = mapper;
            _leaveTypeRepository = leaveTypeRepository;
            _leaveTypeBusinessRules = leaveTypeBusinessRules;
        }

        public async Task<UpdatedLeaveTypeResponse> Handle(UpdateLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            LeaveType? leaveType = await _leaveTypeRepository.GetAsync(predicate: lt => lt.Id == request.Id, cancellationToken: cancellationToken);
            await _leaveTypeBusinessRules.LeaveTypeShouldExistWhenSelected(leaveType);
            leaveType = _mapper.Map(request, leaveType);

            await _leaveTypeRepository.UpdateAsync(leaveType!);

            UpdatedLeaveTypeResponse response = _mapper.Map<UpdatedLeaveTypeResponse>(leaveType);
            return response;
        }
    }
}