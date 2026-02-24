using Application.Features.TimeEntries.Constants;
using Application.Features.TimeEntries.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using MediatR;
using static Application.Features.TimeEntries.Constants.TimeEntriesOperationClaims;

namespace Application.Features.TimeEntries.Queries.GetById;

public class GetByIdTimeEntryQuery : IRequest<GetByIdTimeEntryResponse>, ISecuredRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Read];

    public class GetByIdTimeEntryQueryHandler : IRequestHandler<GetByIdTimeEntryQuery, GetByIdTimeEntryResponse>
    {
        private readonly IMapper _mapper;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly TimeEntryBusinessRules _timeEntryBusinessRules;

        public GetByIdTimeEntryQueryHandler(IMapper mapper, ITimeEntryRepository timeEntryRepository, TimeEntryBusinessRules timeEntryBusinessRules)
        {
            _mapper = mapper;
            _timeEntryRepository = timeEntryRepository;
            _timeEntryBusinessRules = timeEntryBusinessRules;
        }

        public async Task<GetByIdTimeEntryResponse> Handle(GetByIdTimeEntryQuery request, CancellationToken cancellationToken)
        {
            TimeEntry? timeEntry = await _timeEntryRepository.GetAsync(predicate: te => te.Id == request.Id, cancellationToken: cancellationToken);
            await _timeEntryBusinessRules.TimeEntryShouldExistWhenSelected(timeEntry);

            GetByIdTimeEntryResponse response = _mapper.Map<GetByIdTimeEntryResponse>(timeEntry);
            return response;
        }
    }
}