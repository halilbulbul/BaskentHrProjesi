using Application.Features.Timesheets.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Timesheets.Rules;

public class TimesheetBusinessRules : BaseBusinessRules
{
    private readonly ITimesheetRepository _timesheetRepository;
    private readonly ILocalizationService _localizationService;

    public TimesheetBusinessRules(ITimesheetRepository timesheetRepository, ILocalizationService localizationService)
    {
        _timesheetRepository = timesheetRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, TimesheetsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task TimesheetShouldExistWhenSelected(Timesheet? timesheet)
    {
        if (timesheet == null)
            await throwBusinessException(TimesheetsBusinessMessages.TimesheetNotExists);
    }

    public async Task TimesheetIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        Timesheet? timesheet = await _timesheetRepository.GetAsync(
            predicate: t => t.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await TimesheetShouldExistWhenSelected(timesheet);
    }
}