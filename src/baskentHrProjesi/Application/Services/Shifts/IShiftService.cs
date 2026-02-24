using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Shifts;

public interface IShiftservice
{
    Task<Shift?> GetAsync(
        Expression<Func<Shift, bool>> predicate,
        Func<IQueryable<Shift>, IIncludableQueryable<Shift, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<Shift>?> GetListAsync(
        Expression<Func<Shift, bool>>? predicate = null,
        Func<IQueryable<Shift>, IOrderedQueryable<Shift>>? orderBy = null,
        Func<IQueryable<Shift>, IIncludableQueryable<Shift, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<Shift> AddAsync(Shift Shift);
    Task<Shift> UpdateAsync(Shift Shift);
    Task<Shift> DeleteAsync(Shift Shift, bool permanent = false);
}
