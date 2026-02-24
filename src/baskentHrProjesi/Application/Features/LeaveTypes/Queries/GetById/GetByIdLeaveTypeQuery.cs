using Application.Features.LeaveTypes.Constants;
using Application.Features.LeaveTypes.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.LeaveTypes.Constants.LeaveTypesOperationClaims;

namespace Application.Features.LeaveTypes.Queries.GetById;

public class GetByIdLeaveTypeQuery : IRequest<GetByIdLeaveTypeResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read, "Personel"];

    public class GetByIdLeaveTypeQueryHandler : IRequestHandler<GetByIdLeaveTypeQuery, GetByIdLeaveTypeResponse>
    {
        private readonly IMapper _mapper;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly LeaveTypeBusinessRules _leaveTypeBusinessRules;

        public GetByIdLeaveTypeQueryHandler(IMapper mapper, ILeaveTypeRepository leaveTypeRepository, LeaveTypeBusinessRules leaveTypeBusinessRules)
        {
            _mapper = mapper;
            _leaveTypeRepository = leaveTypeRepository;
            _leaveTypeBusinessRules = leaveTypeBusinessRules;
        }

        public async Task<GetByIdLeaveTypeResponse> Handle(GetByIdLeaveTypeQuery request, CancellationToken cancellationToken)
        {
            LeaveType? leaveType = await _leaveTypeRepository.GetAsync(predicate: lt => lt.Id == request.Id, cancellationToken: cancellationToken);
            await _leaveTypeBusinessRules.LeaveTypeShouldExistWhenSelected(leaveType);

            GetByIdLeaveTypeResponse response = _mapper.Map<GetByIdLeaveTypeResponse>(leaveType);
            return response;
        }
    }
}