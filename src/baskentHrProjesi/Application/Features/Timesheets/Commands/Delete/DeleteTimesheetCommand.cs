using Application.Features.Timesheets.Constants;
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

namespace Application.Features.Timesheets.Commands.Delete;

public class DeleteTimesheetCommand : IRequest<DeletedTimesheetResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, TimesheetsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetTimesheets"];

    public class DeleteTimesheetCommandHandler : IRequestHandler<DeleteTimesheetCommand, DeletedTimesheetResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly TimesheetBusinessRules _timesheetBusinessRules;

        public DeleteTimesheetCommandHandler(IMapper mapper, ITimesheetRepository timesheetRepository,
                                         TimesheetBusinessRules timesheetBusinessRules)
        {
            _mapper = mapper;
            _timesheetRepository = timesheetRepository;
            _timesheetBusinessRules = timesheetBusinessRules;
        }

        public async Task<DeletedTimesheetResponse> Handle(DeleteTimesheetCommand request, CancellationToken cancellationToken)
        {
            Timesheet? timesheet = await _timesheetRepository.GetAsync(predicate: t => t.Id == request.Id, cancellationToken: cancellationToken);
            await _timesheetBusinessRules.TimesheetShouldExistWhenSelected(timesheet);

            await _timesheetRepository.DeleteAsync(timesheet!);

            DeletedTimesheetResponse response = _mapper.Map<DeletedTimesheetResponse>(timesheet);
            return response;
        }
    }
}