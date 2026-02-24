using Application.Features.Timesheets.Constants;
using Application.Features.Timesheets.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.Timesheets.Constants.TimesheetsOperationClaims;

namespace Application.Features.Timesheets.Queries.GetById;

public class GetByIdTimesheetQuery : IRequest<GetByIdTimesheetResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdTimesheetQueryHandler : IRequestHandler<GetByIdTimesheetQuery, GetByIdTimesheetResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly TimesheetBusinessRules _timesheetBusinessRules;

        public GetByIdTimesheetQueryHandler(IMapper mapper, ITimesheetRepository timesheetRepository, TimesheetBusinessRules timesheetBusinessRules)
        {
            _mapper = mapper;
            _timesheetRepository = timesheetRepository;
            _timesheetBusinessRules = timesheetBusinessRules;
        }

        public async Task<GetByIdTimesheetResponse> Handle(GetByIdTimesheetQuery request, CancellationToken cancellationToken)
        {
            Timesheet? timesheet = await _timesheetRepository.GetAsync(predicate: t => t.Id == request.Id, cancellationToken: cancellationToken);
            await _timesheetBusinessRules.TimesheetShouldExistWhenSelected(timesheet);

            GetByIdTimesheetResponse response = _mapper.Map<GetByIdTimesheetResponse>(timesheet);
            return response;
        }
    }
}