using Application.Features.EmployeeLeaves.Constants;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Paging;
using static Application.Features.EmployeeLeaves.Constants.EmployeeLeavesOperationClaims;

namespace Application.Features.EmployeeLeaves.Queries.GetListByCurrentUser;

public class GetListByCurrentUserEmployeeLeavesQuery
    : IRequest<GetListResponse<GetListByUserEmployeeLeaveListItemDto>>, ISecuredRequest
{
    public PageRequest PageRequest { get; set; } = null!;
    public Guid UserId { get; set; }
    public string[] Roles => new[] { Admin, Write, "Personel" };

    public class GetListByCurrentUserEmployeeLeavesQueryHandler
        : IRequestHandler<GetListByCurrentUserEmployeeLeavesQuery, GetListResponse<GetListByUserEmployeeLeaveListItemDto>>
    {
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetListByCurrentUserEmployeeLeavesQueryHandler(
            IEmployeeLeaveRepository employeeLeaveRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeLeaveRepository = employeeLeaveRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<GetListResponse<GetListByUserEmployeeLeaveListItemDto>> Handle(
            GetListByCurrentUserEmployeeLeavesQuery request,
            CancellationToken cancellationToken)
        {
            Employee? employee = await _employeeRepository.GetAsync(
                predicate: e => e.UserId == request.UserId,
                enableTracking: false,
                cancellationToken: cancellationToken
            );

            if (employee == null)
            {
                return new GetListResponse<GetListByUserEmployeeLeaveListItemDto>
                {
                    Items = new List<GetListByUserEmployeeLeaveListItemDto>(),
                    Index = request.PageRequest.PageIndex,
                    Size = int.MaxValue,
                    Count = 0,
                    Pages = 0,
                    HasNext = false,
                    HasPrevious = false
                };
            }

            IPaginate<EmployeeLeave> employeeLeaves = await _employeeLeaveRepository.GetListAsync(
                predicate: el => el.EmployeeId == employee.Id,
                include: q => q
                    .Include(el => el.LeaveType)          
                    .Include(el => el.ApproverEmployee), 
                index: request.PageRequest.PageIndex,
                size: int.MaxValue,
                cancellationToken: cancellationToken
            );

            GetListResponse<GetListByUserEmployeeLeaveListItemDto> response =
                _mapper.Map<GetListResponse<GetListByUserEmployeeLeaveListItemDto>>(employeeLeaves);

            return response;
        }
    }
}
