using Application.Features.EmployeeLeaves.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Transaction;
using static Application.Features.EmployeeLeaves.Constants.EmployeeLeavesOperationClaims;

namespace Application.Features.EmployeeLeaves.Commands.Approve;

public class ApproveEmployeeLeaveCommand : IRequest<ApprovedEmployeeLeaveResponse>, ISecuredRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public Guid? ApproverUserId { get; set; }

    public string[] Roles => new[] { Admin, Write };

    public class ApproveEmployeeLeaveCommandHandler : IRequestHandler<ApproveEmployeeLeaveCommand, ApprovedEmployeeLeaveResponse>
    {
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmployeeLeaveBusinessRules _employeeLeaveBusinessRules;
        private readonly IMapper _mapper;

        public ApproveEmployeeLeaveCommandHandler(
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

        public async Task<ApprovedEmployeeLeaveResponse> Handle(ApproveEmployeeLeaveCommand request, CancellationToken cancellationToken)
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

            employeeLeave!.ApprovalStatus = "Onaylandı";

            await _employeeLeaveRepository.UpdateAsync(employeeLeave);

            ApprovedEmployeeLeaveResponse response = _mapper.Map<ApprovedEmployeeLeaveResponse>(employeeLeave);
            return response;
        }
    }
}
