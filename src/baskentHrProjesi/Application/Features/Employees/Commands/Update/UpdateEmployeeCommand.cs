using Application.Features.Employees.Constants;
using Application.Features.Employees.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using static Application.Features.Employees.Constants.EmployeesOperationClaims;

namespace Application.Features.Employees.Commands.Update;

public class UpdateEmployeeCommand : IRequest<UpdatedEmployeeResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required string EmployeeNumber { get; set; }
    public required string IdentityNo { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateTime? BirthDate { get; set; }
    public required string Gender { get; set; }
    public required string MaritalStatus { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public required string AddressLine { get; set; }
    public required string City { get; set; }
    public required string District { get; set; }
    public required string PostalCode { get; set; }
    public required DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public required int DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public int? ShiftId { get; set; }

    public string[] Roles => [Admin, Write, EmployeesOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetEmployees"];

    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, UpdatedEmployeeResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmployeeBusinessRules _employeeBusinessRules;
        private readonly IUserRepository _userRepository;

        public UpdateEmployeeCommandHandler(
            IMapper mapper,
            IEmployeeRepository employeeRepository,
            EmployeeBusinessRules employeeBusinessRules,
            IUserRepository userRepository)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _employeeBusinessRules = employeeBusinessRules;
            _userRepository = userRepository;
        }

        public async Task<UpdatedEmployeeResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            Employee? employee =
                await _employeeRepository.GetAsync(e => e.Id == request.Id, cancellationToken: cancellationToken);
            await _employeeBusinessRules.EmployeeShouldExistWhenSelected(employee);

            employee = _mapper.Map(request, employee);
            await _employeeRepository.UpdateAsync(employee!);

            if (employee!.UserId.HasValue)
            {
                var user = await _userRepository.GetAsync(
                    u => u.Id == employee.UserId.Value,
                    cancellationToken: cancellationToken);

                if (user != null)
                {
                    user.Email = employee.Email;
                    await _userRepository.UpdateAsync(user);
                }
            }

            UpdatedEmployeeResponse response = _mapper.Map<UpdatedEmployeeResponse>(employee);
            return response;
        }
    }
}
