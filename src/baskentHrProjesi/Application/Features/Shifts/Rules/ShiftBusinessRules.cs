using Application.Features.Shifts.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Shifts.Rules;

public class ShiftBusinessRules : BaseBusinessRules
{
    private readonly IShiftRepository _ShiftRepository;
    private readonly ILocalizationService _localizationService;

    public ShiftBusinessRules(IShiftRepository ShiftRepository, ILocalizationService localizationService)
    {
        _ShiftRepository = ShiftRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, ShiftsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task ShiftshouldExistWhenSelected(Shift? Shift)
    {
        if (Shift == null)
            await throwBusinessException(ShiftsBusinessMessages.ShiftNotExists);
    }

    public async Task ShiftIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        Shift? Shift = await _ShiftRepository.GetAsync(
            predicate: trs => trs.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await ShiftshouldExistWhenSelected(Shift);
    }
}