using Application.Features.TimeEntries.Constants;
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

namespace Application.Features.TimeEntries.Commands.Delete;

public class DeleteTimeEntryCommand : IRequest<DeletedTimeEntryResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, TimeEntriesOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetTimeEntries"];

    public class DeleteTimeEntryCommandHandler : IRequestHandler<DeleteTimeEntryCommand, DeletedTimeEntryResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly TimeEntryBusinessRules _timeEntryBusinessRules;

        public DeleteTimeEntryCommandHandler(IMapper mapper, ITimeEntryRepository timeEntryRepository,
                                         TimeEntryBusinessRules timeEntryBusinessRules)
        {
            _mapper = mapper;
            _timeEntryRepository = timeEntryRepository;
            _timeEntryBusinessRules = timeEntryBusinessRules;
        }

        public async Task<DeletedTimeEntryResponse> Handle(DeleteTimeEntryCommand request, CancellationToken cancellationToken)
        {
            TimeEntry? timeEntry = await _timeEntryRepository.GetAsync(predicate: te => te.Id == request.Id, cancellationToken: cancellationToken);
            await _timeEntryBusinessRules.TimeEntryShouldExistWhenSelected(timeEntry);

            await _timeEntryRepository.DeleteAsync(timeEntry!);

            DeletedTimeEntryResponse response = _mapper.Map<DeletedTimeEntryResponse>(timeEntry);
            return response;
        }
    }
}