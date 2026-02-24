using Application.Features.LeaveTypes.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.LeaveTypes;

public class LeaveTypeManager : ILeaveTypeService
{
    private readonly ILeaveTypeRepository _leaveTypeRepository;
    private readonly LeaveTypeBusinessRules _leaveTypeBusinessRules;

    public LeaveTypeManager(ILeaveTypeRepository leaveTypeRepository, LeaveTypeBusinessRules leaveTypeBusinessRules)
    {
        _leaveTypeRepository = leaveTypeRepository;
        _leaveTypeBusinessRules = leaveTypeBusinessRules;
    }

    public async Task<LeaveType?> GetAsync(
        Expression<Func<LeaveType, bool>> predicate,
        Func<IQueryable<LeaveType>, IIncludableQueryable<LeaveType, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        LeaveType? leaveType = await _leaveTypeRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return leaveType;
    }

    public async Task<IPaginate<LeaveType>?> GetListAsync(
        Expression<Func<LeaveType, bool>>? predicate = null,
        Func<IQueryable<LeaveType>, IOrderedQueryable<LeaveType>>? orderBy = null,
        Func<IQueryable<LeaveType>, IIncludableQueryable<LeaveType, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<LeaveType> leaveTypeList = await _leaveTypeRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return leaveTypeList;
    }

    public async Task<LeaveType> AddAsync(LeaveType leaveType)
    {
        LeaveType addedLeaveType = await _leaveTypeRepository.AddAsync(leaveType);

        return addedLeaveType;
    }

    public async Task<LeaveType> UpdateAsync(LeaveType leaveType)
    {
        LeaveType updatedLeaveType = await _leaveTypeRepository.UpdateAsync(leaveType);

        return updatedLeaveType;
    }

    public async Task<LeaveType> DeleteAsync(LeaveType leaveType, bool permanent = false)
    {
        LeaveType deletedLeaveType = await _leaveTypeRepository.DeleteAsync(leaveType);

        return deletedLeaveType;
    }
}
