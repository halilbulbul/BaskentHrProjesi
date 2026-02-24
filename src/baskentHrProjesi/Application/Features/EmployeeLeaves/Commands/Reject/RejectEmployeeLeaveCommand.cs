using Application.Features.EmployeeLeaves.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Transaction;
using static Application.Features.EmployeeLeaves.Constants.EmployeeLeavesOperationClaims;

namespace Application.Features.EmployeeLeaves.Commands.Reject;

public class RejectEmployeeLeaveCommand : IRequest<RejectedEmployeeLeaveResponse>, ISecuredRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public Guid? ApproverUserId { get; set; }

    public string[] Roles => new[] { Admin, Write };

    public class RejectEmployeeLeaveCommandHandler : IRequestHandler<RejectEmployeeLeaveCommand, RejectedEmployeeLeaveResponse>
    {
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmployeeLeaveBusinessRules _employeeLeaveBusinessRules;
        private readonly IMapper _mapper;

        public RejectEmployeeLeaveCommandHandler(
            IEmployeeLeaveRepository employeeLeaveRepository,
            IEmployeeRepository employeeRepository,
            EmployeeLeaveBusinessRules employeeLeaveBusinessRules,
            IMapper mapper)
        {
            _employeeLeaveRepository = employeeLeaveRepository;
            _employeeRepository = employeeRepository;
            _employeeLeaveBusinessRules = employeeLeaveBusinessRules;
            _mapper = mapper;
        }

        public async Task<RejectedEmployeeLeaveResponse> Handle(RejectEmployeeLeaveCommand request, CancellationToken cancellationToken)
        {
            EmployeeLeave? employeeLeave = await _employeeLeaveRepository.GetAsync(
                e => e.Id == request.Id,
                cancellationToken: cancellationToken);

            await _employeeLeaveBusinessRules.EmployeeLeaveShouldExistWhenSelected(employeeLeave);

            if (request.ApproverUserId.HasValue)
            {
                var approver = await _employeeRepository.GetAsync(
                    e => e.UserId == request.ApproverUserId.Value,
                    cancellationToken: cancellationToken);

                if (approver != null)
                    employeeLeave!.ApproverEmployeeId = approver.Id;
            }

            employeeLeave!.ApprovalStatus = "Reddedildi";

            await _employeeLeaveRepository.UpdateAsync(employeeLeave);

            RejectedEmployeeLeaveResponse response = _mapper.Map<RejectedEmployeeLeaveResponse>(employeeLeave);
            return response;
        }
    }
}
