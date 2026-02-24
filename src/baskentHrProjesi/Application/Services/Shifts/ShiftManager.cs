using Application.Features.Shifts.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Shifts;

public class ShiftManager : IShiftservice
{
    private readonly IShiftRepository _ShiftRepository;
    private readonly ShiftBusinessRules _ShiftBusinessRules;

    public ShiftManager(IShiftRepository ShiftRepository, ShiftBusinessRules ShiftBusinessRules)
    {
        _ShiftRepository = ShiftRepository;
        _ShiftBusinessRules = ShiftBusinessRules;
    }

    public async Task<Shift?> GetAsync(
        Expression<Func<Shift, bool>> predicate,
        Func<IQueryable<Shift>, IIncludableQueryable<Shift, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Shift? Shift = await _ShiftRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return Shift;
    }

    public async Task<IPaginate<Shift>?> GetListAsync(
        Expression<Func<Shift, bool>>? predicate = null,
        Func<IQueryable<Shift>, IOrderedQueryable<Shift>>? orderBy = null,
        Func<IQueryable<Shift>, IIncludableQueryable<Shift, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Shift> ShiftList = await _ShiftRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return ShiftList;
    }

    public async Task<Shift> AddAsync(Shift Shift)
    {
        Shift addedShift = await _ShiftRepository.AddAsync(Shift);

        return addedShift;
    }

    public async Task<Shift> UpdateAsync(Shift Shift)
    {
        Shift updatedShift = await _ShiftRepository.UpdateAsync(Shift);

        return updatedShift;
    }

    public async Task<Shift> DeleteAsync(Shift Shift, bool permanent = false)
    {
        Shift deletedShift = await _ShiftRepository.DeleteAsync(Shift);

        return deletedShift;
    }
}
