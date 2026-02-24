using Application.Features.Employees.Constants;
using Application.Features.Employees.Rules;
using Application.Features.Users.Commands.Create;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Employees.Constants.EmployeesOperationClaims;

namespace Application.Features.Employees.Commands.Create;

public class CreateEmployeeCommand : IRequest<CreatedEmployeeResponse>, ISecuredRequest, ITransactionalRequest
{
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

    public string[] Roles => [Admin, Write, EmployeesOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetEmployees"];

    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, CreatedEmployeeResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmployeeBusinessRules _employeeBusinessRules;
        private readonly IMediator _mediator;

        public CreateEmployeeCommandHandler(
            IMapper mapper,
            IEmployeeRepository employeeRepository,
            EmployeeBusinessRules employeeBusinessRules,
            IMediator mediator)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _employeeBusinessRules = employeeBusinessRules;
            _mediator = mediator;
        }

        public async Task<CreatedEmployeeResponse> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            Employee employee = _mapper.Map<Employee>(request);
            employee = await _employeeRepository.AddAsync(employee);

            var createUserCommand = new CreateUserCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Password = request.IdentityNo
            };

            var createdUser = await _mediator.Send(createUserCommand, cancellationToken);

            employee.UserId = createdUser.Id;
            await _employeeRepository.UpdateAsync(employee);

            CreatedEmployeeResponse response = _mapper.Map<CreatedEmployeeResponse>(employee);
            return response;
        }
    }
}
