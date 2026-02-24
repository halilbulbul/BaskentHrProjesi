using Application.Features.Timesheets.Constants;
using Application.Features.Timesheets.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Timesheets.Constants.TimesheetsOperationClaims;

namespace Application.Features.Timesheets.Commands.Create;

public class CreateTimesheetCommand : IRequest<CreatedTimesheetResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required int EmployeeId { get; set; }
    public required DateTime WorkDate { get; set; }
    public required int ShiftId { get; set; }
    public required int PlannedMinutes { get; set; }
    public required int ActualMinutes { get; set; }
    public required int OvertimeMinutes { get; set; }
    public required int MissingMinutes { get; set; }
    public required int LateArrivalMinutes { get; set; }
    public required int EarlyLeaveMinutes { get; set; }
    public required int EarlyArrivalMinutes { get; set; }
    public required int LateLeaveMinutes { get; set; }
    public required string Status { get; set; }
    public int? EmployeeLeaveId { get; set; }

    public string[] Roles => [Admin, Write, TimesheetsOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetTimesheets"];

    public class CreateTimesheetCommandHandler : IRequestHandler<CreateTimesheetCommand, CreatedTimesheetResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly TimesheetBusinessRules _timesheetBusinessRules;

        public CreateTimesheetCommandHandler(IMapper mapper, ITimesheetRepository timesheetRepository,
                                         TimesheetBusinessRules timesheetBusinessRules)
        {
            _mapper = mapper;
            _timesheetRepository = timesheetRepository;
            _timesheetBusinessRules = timesheetBusinessRules;
        }

        public async Task<CreatedTimesheetResponse> Handle(CreateTimesheetCommand request, CancellationToken cancellationToken)
        {
            Timesheet timesheet = _mapper.Map<Timesheet>(request);

            await _timesheetRepository.AddAsync(timesheet);

            CreatedTimesheetResponse response = _mapper.Map<CreatedTimesheetResponse>(timesheet);
            return response;
        }
    }
}