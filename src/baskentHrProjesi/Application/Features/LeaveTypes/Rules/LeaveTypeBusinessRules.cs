using Application.Features.LeaveTypes.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.LeaveTypes.Rules;

public class LeaveTypeBusinessRules : BaseBusinessRules
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly ILocalizationService _localizationService;

    public LeaveTypeBusinessRules(ILeaveTypeRepository leaveTypeRepository, ILocalizationService localizationService)
    {
        _leaveTypeRepository = leaveTypeRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, LeaveTypesBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task LeaveTypeShouldExistWhenSelected(LeaveType? leaveType)
    {
        if (leaveType == null)
            await throwBusinessException(LeaveTypesBusinessMessages.LeaveTypeNotExists);
    }

    public async Task LeaveTypeIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        LeaveType? leaveType = await _leaveTypeRepository.GetAsync(
            predicate: lt => lt.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await LeaveTypeShouldExistWhenSelected(leaveType);
    }
}