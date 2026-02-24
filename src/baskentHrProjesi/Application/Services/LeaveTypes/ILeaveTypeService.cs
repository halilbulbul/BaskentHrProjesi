using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.LeaveTypes;

public interface ILeaveTypeService
{
    Task<LeaveType?> GetAsync(
        Expression<Func<LeaveType, bool>> predicate,
        Func<IQueryable<LeaveType>, IIncludableQueryable<LeaveType, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<LeaveType>?> GetListAsync(
        Expression<Func<LeaveType, bool>>? predicate = null,
        Func<IQueryable<LeaveType>, IOrderedQueryable<LeaveType>>? orderBy = null,
        Func<IQueryable<LeaveType>, IIncludableQueryable<LeaveType, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<LeaveType> AddAsync(LeaveType leaveType);
    Task<LeaveType> UpdateAsync(LeaveType leaveType);
    Task<LeaveType> DeleteAsync(LeaveType leaveType, bool permanent = false);
}
