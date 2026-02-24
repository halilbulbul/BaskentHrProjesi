using Application.Features.Shifts.Constants;
using Application.Features.Shifts.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Shifts.Constants.ShiftsOperationClaims;

namespace Application.Features.Shifts.Queries.GetById;

public class GetByIdShiftQuery : IRequest<GetByIdShiftResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdShiftQueryHandler : IRequestHandler<GetByIdShiftQuery, GetByIdShiftResponse>
    {
        private readonly IMapper _mapper;
        private readonly IShiftRepository _ShiftRepository;
        private readonly ShiftBusinessRules _ShiftBusinessRules;

        public GetByIdShiftQueryHandler(IMapper mapper, IShiftRepository ShiftRepository, ShiftBusinessRules ShiftBusinessRules)
        {
            _mapper = mapper;
            _ShiftRepository = ShiftRepository;
            _ShiftBusinessRules = ShiftBusinessRules;
        }

        public async Task<GetByIdShiftResponse> Handle(GetByIdShiftQuery request, CancellationToken cancellationToken)
        {
            Shift? Shift = await _ShiftRepository.GetAsync(predicate: trs => trs.Id == request.Id, cancellationToken: cancellationToken);
            await _ShiftBusinessRules.ShiftshouldExistWhenSelected(Shift);

            GetByIdShiftResponse response = _mapper.Map<GetByIdShiftResponse>(Shift);
            return response;
        }
    }
}