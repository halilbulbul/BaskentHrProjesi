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

namespace Application.Features.TimeEntries.Commands.Create;

public class CreateTimeEntryCommand : IRequest<CreatedTimeEntryResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required int EmployeeId { get; set; }
    public required DateTime EventTime { get; set; }
    public required string Direction { get; set; }
    public required string Source { get; set; }
    public required string DeviceId { get; set; }

    public string[] Roles => [Admin, Write, TimeEntriesOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetTimeEntries"];

    public class CreateTimeEntryCommandHandler : IRequestHandler<CreateTimeEntryCommand, CreatedTimeEntryResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly TimeEntryBusinessRules _timeEntryBusinessRules;

        public CreateTimeEntryCommandHandler(IMapper mapper, ITimeEntryRepository timeEntryRepository,
                                         TimeEntryBusinessRules timeEntryBusinessRules)
        {
            _mapper = mapper;
            _timeEntryRepository = timeEntryRepository;
            _timeEntryBusinessRules = timeEntryBusinessRules;
        }

        public async Task<CreatedTimeEntryResponse> Handle(CreateTimeEntryCommand request, CancellationToken cancellationToken)
        {
            TimeEntry timeEntry = _mapper.Map<TimeEntry>(request);

            await _timeEntryRepository.AddAsync(timeEntry);

            CreatedTimeEntryResponse response = _mapper.Map<CreatedTimeEntryResponse>(timeEntry);
            return response;
        }
    }
}