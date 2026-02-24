using Application.Features.EmployeeLeaves.Constants;
using Application.Features.EmployeeLeaves.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.EmployeeLeaves.Constants.EmployeeLeavesOperationClaims;

namespace Application.Features.EmployeeLeaves.Queries.GetById;

public class GetByIdEmployeeLeaveQuery : IRequest<GetByIdEmployeeLeaveResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdEmployeeLeaveQueryHandler : IRequestHandler<GetByIdEmployeeLeaveQuery, GetByIdEmployeeLeaveResponse>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly EmployeeLeaveBusinessRules _employeeLeaveBusinessRules;

        public GetByIdEmployeeLeaveQueryHandler(IMapper mapper, IEmployeeLeaveRepository employeeLeaveRepository, EmployeeLeaveBusinessRules employeeLeaveBusinessRules)
        {
            _mapper = mapper;
            _employeeLeaveRepository = employeeLeaveRepository;
            _employeeLeaveBusinessRules = employeeLeaveBusinessRules;
        }

        public async Task<GetByIdEmployeeLeaveResponse> Handle(GetByIdEmployeeLeaveQuery request, CancellationToken cancellationToken)
        {
            EmployeeLeave? employeeLeave = await _employeeLeaveRepository.GetAsync(predicate: el => el.Id == request.Id, cancellationToken: cancellationToken);
            await _employeeLeaveBusinessRules.EmployeeLeaveShouldExistWhenSelected(employeeLeave);

            GetByIdEmployeeLeaveResponse response = _mapper.Map<GetByIdEmployeeLeaveResponse>(employeeLeave);
            return response;
        }
    }
}