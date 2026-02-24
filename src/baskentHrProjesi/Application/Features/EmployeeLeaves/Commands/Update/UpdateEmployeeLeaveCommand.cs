using Application.Features.EmployeeLeaves.Constants;
using Application.Features.EmployeeLeaves.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.EmployeeLeaves.Constants.EmployeeLeavesOperationClaims;

namespace Application.Features.EmployeeLeaves.Commands.Update;

public class UpdateEmployeeLeaveCommand : IRequest<UpdatedEmployeeLeaveResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required int EmployeeId { get; set; }
    public required int LeaveTypeId { get; set; }
    public required DateTime StartDate { get; set; }
    public required DateTime EndDate { get; set; }
    public required int TotalDays { get; set; }
    public required string? ApprovalStatus { get; set; }
    public required DateTime RequestDate { get; set; }
    public int? ApproverEmployeeId { get; set; }

    public string[] Roles => [Admin, Write, EmployeeLeavesOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetEmployeeLeaves"];

    public class UpdateEmployeeLeaveCommandHandler : IRequestHandler<UpdateEmployeeLeaveCommand, UpdatedEmployeeLeaveResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly EmployeeLeaveBusinessRules _employeeLeaveBusinessRules;

        public UpdateEmployeeLeaveCommandHandler(IMapper mapper, IEmployeeLeaveRepository employeeLeaveRepository,
                                         EmployeeLeaveBusinessRules employeeLeaveBusinessRules)
        {
            _mapper = mapper;
            _employeeLeaveRepository = employeeLeaveRepository;
            _employeeLeaveBusinessRules = employeeLeaveBusinessRules;
        }

        public async Task<UpdatedEmployeeLeaveResponse> Handle(UpdateEmployeeLeaveCommand request, CancellationToken cancellationToken)
        {
            EmployeeLeave? employeeLeave = await _employeeLeaveRepository.GetAsync(predicate: el => el.Id == request.Id, cancellationToken: cancellationToken);
            await _employeeLeaveBusinessRules.EmployeeLeaveShouldExistWhenSelected(employeeLeave);
            employeeLeave = _mapper.Map(request, employeeLeave);

            await _employeeLeaveRepository.UpdateAsync(employeeLeave!);

            UpdatedEmployeeLeaveResponse response = _mapper.Map<UpdatedEmployeeLeaveResponse>(employeeLeave);
            return response;
        }
    }
}