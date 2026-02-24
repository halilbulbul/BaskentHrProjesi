using Application.Features.LeaveTypes.Constants;
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

namespace Application.Features.LeaveTypes.Commands.Delete;

public class DeleteLeaveTypeCommand : IRequest<DeletedLeaveTypeResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, LeaveTypesOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetLeaveTypes"];

    public class DeleteLeaveTypeCommandHandler : IRequestHandler<DeleteLeaveTypeCommand, DeletedLeaveTypeResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly LeaveTypeBusinessRules _leaveTypeBusinessRules;

        public DeleteLeaveTypeCommandHandler(IMapper mapper, ILeaveTypeRepository leaveTypeRepository,
                                         LeaveTypeBusinessRules leaveTypeBusinessRules)
        {
            _mapper = mapper;
            _leaveTypeRepository = leaveTypeRepository;
            _leaveTypeBusinessRules = leaveTypeBusinessRules;
        }

        public async Task<DeletedLeaveTypeResponse> Handle(DeleteLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            LeaveType? leaveType = await _leaveTypeRepository.GetAsync(predicate: lt => lt.Id == request.Id, cancellationToken: cancellationToken);
            await _leaveTypeBusinessRules.LeaveTypeShouldExistWhenSelected(leaveType);

            await _leaveTypeRepository.DeleteAsync(leaveType!);

            DeletedLeaveTypeResponse response = _mapper.Map<DeletedLeaveTypeResponse>(leaveType);
            return response;
        }
    }
}