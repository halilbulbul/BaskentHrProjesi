using Application.Features.EmployeeLeaves.Constants;
using Application.Services.Repositories;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.CrossCuttingConcerns.Exception.Types;
using NArchitecture.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.EmployeeLeaves.Rules;

public class EmployeeLeaveBusinessRules : BaseBusinessRules
{
    private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
    private readonly ILocalizationService _localizationService;

    public EmployeeLeaveBusinessRules(IEmployeeLeaveRepository employeeLeaveRepository, ILocalizationService localizationService)
    {
        _employeeLeaveRepository = employeeLeaveRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, EmployeeLeavesBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task EmployeeLeaveShouldExistWhenSelected(EmployeeLeave? employeeLeave)
    {
        if (employeeLeave == null)
            await throwBusinessException(EmployeeLeavesBusinessMessages.EmployeeLeaveNotExists);
    }

    public async Task EmployeeLeaveIdShouldExistWhenSelected(int id, CancellationToken cancellationToken)
    {
        EmployeeLeave? employeeLeave = await _employeeLeaveRepository.GetAsync(
            predicate: el => el.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await EmployeeLeaveShouldExistWhenSelected(employeeLeave);
    }
}