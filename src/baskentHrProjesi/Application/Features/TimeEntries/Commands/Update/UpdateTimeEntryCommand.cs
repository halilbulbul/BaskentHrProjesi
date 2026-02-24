using Application.Features.TimeEntries.Constants;
using Application.Features.TimeEntries.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.TimeEntries.Constants.TimeEntriesOperationClaims;

namespace Application.Features.TimeEntries.Commands.Update;

public class UpdateTimeEntryCommand : IRequest<UpdatedTimeEntryResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }
    public required int EmployeeId { get; set; }
    public required DateTime EventTime { get; set; }
    public required string Direction { get; set; }
    public required string Source { get; set; }
    public required string DeviceId { get; set; }

    public string[] Roles => [Admin, Write, TimeEntriesOperationClaims.Update];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetTimeEntries"];

    public class UpdateTimeEntryCommandHandler : IRequestHandler<UpdateTimeEntryCommand, UpdatedTimeEntryResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly TimeEntryBusinessRules _timeEntryBusinessRules;

        public UpdateTimeEntryCommandHandler(IMapper mapper, ITimeEntryRepository timeEntryRepository,
                                         TimeEntryBusinessRules timeEntryBusinessRules)
        {
            _mapper = mapper;
            _timeEntryRepository = timeEntryRepository;
            _timeEntryBusinessRules = timeEntryBusinessRules;
        }

        public async Task<UpdatedTimeEntryResponse> Handle(UpdateTimeEntryCommand request, CancellationToken cancellationToken)
        {
            TimeEntry? timeEntry = await _timeEntryRepository.GetAsync(predicate: te => te.Id == request.Id, cancellationToken: cancellationToken);
            await _timeEntryBusinessRules.TimeEntryShouldExistWhenSelected(timeEntry);
            timeEntry = _mapper.Map(request, timeEntry);

            await _timeEntryRepository.UpdateAsync(timeEntry!);

            UpdatedTimeEntryResponse response = _mapper.Map<UpdatedTimeEntryResponse>(timeEntry);
            return response;
        }
    }
}