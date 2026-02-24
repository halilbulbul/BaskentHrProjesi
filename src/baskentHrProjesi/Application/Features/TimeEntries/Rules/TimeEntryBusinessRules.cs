using Application.Features.TimeEntries.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.TimeEntries.Rules;

public class TimeEntryBusinessRules : BaseBusinessRules
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly ILocalizationService _localizationService;

    public TimeEntryBusinessRules(ITimeEntryRepository timeEntryRepository, ILocalizationService localizationService)
    {
        _timeEntryRepository = timeEntryRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, TimeEntriesBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task TimeEntryShouldExistWhenSelected(TimeEntry? timeEntry)
    {
        if (timeEntry == null)
            await throwBusinessException(TimeEntriesBusinessMessages.TimeEntryNotExists);
    }

    public async Task TimeEntryIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        TimeEntry? timeEntry = await _timeEntryRepository.GetAsync(
            predicate: te => te.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await TimeEntryShouldExistWhenSelected(timeEntry);
    }
}