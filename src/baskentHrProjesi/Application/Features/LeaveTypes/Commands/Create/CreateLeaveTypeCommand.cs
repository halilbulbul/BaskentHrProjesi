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

namespace Application.Features.LeaveTypes.Commands.Create;

public class CreateLeaveTypeCommand : IRequest<CreatedLeaveTypeResponse>, ISecuredRequest, ICacheRemoverRequest
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required bool IsPaid { get; set; }

    public string[] Roles => [Admin, Write, LeaveTypesOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetLeaveTypes"];

    public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, CreatedLeaveTypeResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly LeaveTypeBusinessRules _leaveTypeBusinessRules;

        public CreateLeaveTypeCommandHandler(IMapper mapper, ILeaveTypeRepository leaveTypeRepository,
                                         LeaveTypeBusinessRules leaveTypeBusinessRules)
        {
            _mapper = mapper;
            _leaveTypeRepository = leaveTypeRepository;
            _leaveTypeBusinessRules = leaveTypeBusinessRules;
        }

        public async Task<CreatedLeaveTypeResponse> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            LeaveType leaveType = _mapper.Map<LeaveType>(request);

            await _leaveTypeRepository.AddAsync(leaveType);

            CreatedLeaveTypeResponse response = _mapper.Map<CreatedLeaveTypeResponse>(leaveType);
            return response;
        }
    }
}