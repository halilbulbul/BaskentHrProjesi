using Application.Features.EmployeeLeaves.Constants;
using Application.Features.EmployeeLeaves.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.EmployeeLeaves.Constants.EmployeeLeavesOperationClaims;

namespace Application.Features.EmployeeLeaves.Commands.Create;

public class CreateEmployeeLeaveCommand : IRequest<CreatedEmployeeLeaveResponse>, ISecuredRequest, ITransactionalRequest
{
    public int? EmployeeId { get; set; }
    public int? LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? TotalDays { get; set; }
    public string? ApprovalStatus { get; set; }
    public DateTime RequestDate { get; set; }
    public int? ApproverEmployeeId { get; set; }
    public Guid? EmployeeUserId { get; set; }

    public string[] Roles => new[] { Admin, Write, "Personel" };
    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetEmployeeLeaves"];
    public class CreateEmployeeLeaveCommandHandler : IRequestHandler<CreateEmployeeLeaveCommand, CreatedEmployeeLeaveResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly EmployeeLeaveBusinessRules _employeeLeaveBusinessRules;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateEmployeeLeaveCommandHandler(
            IMapper mapper,
            IEmployeeLeaveRepository employeeLeaveRepository,
            EmployeeLeaveBusinessRules employeeLeaveBusinessRules,
            IEmployeeRepository employeeRepository)
        {
            _mapper = mapper;
            _employeeLeaveRepository = employeeLeaveRepository;
            _employeeLeaveBusinessRules = employeeLeaveBusinessRules;
            _employeeRepository = employeeRepository;
        }

        public async Task<CreatedEmployeeLeaveResponse> Handle(CreateEmployeeLeaveCommand request, CancellationToken cancellationToken)
        {
            if (!request.TotalDays.HasValue)
                request.TotalDays = (int)(request.EndDate.Date - request.StartDate.Date).TotalDays + 1;

            if (request.RequestDate == default)
                request.RequestDate = DateTime.UtcNow;

            if (!request.EmployeeId.HasValue && request.EmployeeUserId.HasValue)
            {
                Employee? employee = await _employeeRepository.GetAsync(
                    e => e.UserId == request.EmployeeUserId.Value,
                    cancellationToken: cancellationToken);

                if (employee != null)
                    request.EmployeeId = employee.Id;
            }

            EmployeeLeave employeeLeave = _mapper.Map<EmployeeLeave>(request);

            await _employeeLeaveRepository.AddAsync(employeeLeave);

            CreatedEmployeeLeaveResponse response = _mapper.Map<CreatedEmployeeLeaveResponse>(employeeLeave);
            return response;
        }
    }
}
