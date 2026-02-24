using Application.Features.EmployeeLeaves.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using MediatR;
using static Application.Features.EmployeeLeaves.Constants.EmployeeLeavesOperationClaims;

namespace Application.Features.EmployeeLeaves.Queries.GetList;

public class GetListEmployeeLeaveQuery : IRequest<GetListResponse<GetListEmployeeLeaveListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; }

    public string[] Roles => [Admin, Read];

    public bool BypassCache { get; }
    public string? CacheKey => $"GetListEmployeeLeaves({PageRequest.PageIndex},{PageRequest.PageSize})";
    public string? CacheGroupKey => "GetEmployeeLeaves";
    public TimeSpan? SlidingExpiration { get; }

    public class GetListEmployeeLeaveQueryHandler : IRequestHandler<GetListEmployeeLeaveQuery, GetListResponse<GetListEmployeeLeaveListItemDto>>
    {
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly IMapper _mapper;

        public GetListEmployeeLeaveQueryHandler(IEmployeeLeaveRepository employeeLeaveRepository, IMapper mapper)
        {
            _employeeLeaveRepository = employeeLeaveRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListEmployeeLeaveListItemDto>> Handle(GetListEmployeeLeaveQuery request, CancellationToken cancellationToken)
        {
            IPaginate<EmployeeLeave> employeeLeaves = await _employeeLeaveRepository.GetListAsync(
                index: request.PageRequest.PageIndex,
                size: int.MaxValue, 
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListEmployeeLeaveListItemDto> response = _mapper.Map<GetListResponse<GetListEmployeeLeaveListItemDto>>(employeeLeaves);
            return response;
        }
    }
}