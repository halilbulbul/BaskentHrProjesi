using Application.Features.UserOperationClaims.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.UserOperationClaims.Queries.GetList;

public class GetListUserOperationClaimQuery : IRequest<GetListResponse<GetListUserOperationClaimListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [UserOperationClaimsOperationClaims.Read];

    public GetListUserOperationClaimQuery()
    {
        PageRequest = new PageRequest { PageIndex = 0, PageSize = 10 };
    }

    public GetListUserOperationClaimQuery(PageRequest pageRequest)
    {
        PageRequest = pageRequest;
    }

    public class GetListUserOperationClaimQueryHandler
        : IRequestHandler<GetListUserOperationClaimQuery, GetListResponse<GetListUserOperationClaimListItemDto>>
    {
        private readonly IUserOperationClaimRepository _userOperationClaimRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetListUserOperationClaimQueryHandler(
            IUserOperationClaimRepository userOperationClaimRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _userOperationClaimRepository = userOperationClaimRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListUserOperationClaimListItemDto>> Handle(
            GetListUserOperationClaimQuery request,
            CancellationToken cancellationToken)
        {
            IPaginate<UserOperationClaim> userOperationClaims = await _userOperationClaimRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: int.MaxValue,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            var userIds = userOperationClaims.Items
                .Select(x => x.UserId)
                .Distinct()
                .ToList();

            IPaginate<Employee> employees = await _employeeRepository.GetListAsync(
                predicate: e => e.UserId != null && userIds.Contains(e.UserId.Value),
                size: int.MaxValue,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            var employeeByUserId = employees.Items
                .GroupBy(e => e.UserId)
                .ToDictionary(g => g.Key, g => g.First());

            var items = userOperationClaims.Items
                .Select(uoc =>
                {
                    employeeByUserId.TryGetValue(uoc.UserId, out var emp);

                    return new GetListUserOperationClaimListItemDto
                    {
                        Id = uoc.Id,
                        UserId = uoc.UserId,
                        OperationClaimId = uoc.OperationClaimId,
                        EmployeeId = emp?.Id,
                        EmployeeNumber = emp?.EmployeeNumber,
                        EmployeeFirstName = emp?.FirstName,
                        EmployeeLastName = emp?.LastName
                    };
                })
                .ToList();

            return new GetListResponse<GetListUserOperationClaimListItemDto>
            {
                Index = userOperationClaims.Index,
                Size = userOperationClaims.Size,
                Count = userOperationClaims.Count,
                Pages = userOperationClaims.Pages,
                HasPrevious = userOperationClaims.HasPrevious,
                HasNext = userOperationClaims.HasNext,
                Items = items
            };
        }
    }
}
