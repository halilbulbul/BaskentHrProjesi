using Application.Features.UserOperationClaims.Constants;
using Application.Features.UserOperationClaims.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;

namespace Application.Features.UserOperationClaims.Queries.GetById;

public class GetByIdUserOperationClaimQuery : IRequest<GetByIdUserOperationClaimResponse>, ISecuredRequest
{
    public Guid Id { get; set; }

    public string[] Roles => [UserOperationClaimsOperationClaims.Read];

    public class GetByIdUserOperationClaimQueryHandler
        : IRequestHandler<GetByIdUserOperationClaimQuery, GetByIdUserOperationClaimResponse>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;
        private readonly UserOperationClaimBusinessRules _userOperationClaimBusinessRules;

        public GetByIdUserOperationClaimQueryHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper,
            UserOperationClaimBusinessRules userOperationClaimBusinessRules
        )
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
            _userOperationClaimBusinessRules = userOperationClaimBusinessRules;
        }

        public async Task<GetByIdUserOperationClaimResponse> Handle(
            GetByIdUserOperationClaimQuery request,
            CancellationToken cancellationToken
        )
        {
            UserOperationClaim? userOperationClaim = await _userOperationClaimRepository.GetAsync(
                predicate: b => b.Id.Equals(request.Id),
                enableTracking: false,
                cancellationToken: cancellationToken
            );
            await _userOperationClaimBusinessRules.UserOperationClaimShouldExistWhenSelected(userOperationClaim);

            GetByIdUserOperationClaimResponse response =
                _mapper.Map<GetByIdUserOperationClaimResponse>(userOperationClaim);

            Employee? employee = await _employeeRepository.GetAsync(
                e => e.UserId == userOperationClaim!.UserId,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            if (employee != null)
            {
                response.EmployeeId = employee.Id;
                response.EmployeeNumber = employee.EmployeeNumber;
                response.EmployeeFirstName = employee.FirstName;
                response.EmployeeLastName = employee.LastName;
            }

            return response;
        }
    }
}
