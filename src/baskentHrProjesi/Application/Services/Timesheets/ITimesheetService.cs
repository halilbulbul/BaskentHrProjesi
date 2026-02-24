using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Timesheets;

public interface ITimesheetService
{
    Task<Timesheet?> GetAsync(
        Expression<Func<Timesheet, bool>> predicate,
        Func<IQueryable<Timesheet>, IIncludableQueryable<Timesheet, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<Timesheet>?> GetListAsync(
        Expression<Func<Timesheet, bool>>? predicate = null,
        Func<IQueryable<Timesheet>, IOrderedQueryable<Timesheet>>? orderBy = null,
        Func<IQueryable<Timesheet>, IIncludableQueryable<Timesheet, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<Timesheet> AddAsync(Timesheet timesheet);
    Task<Timesheet> UpdateAsync(Timesheet timesheet);
    Task<Timesheet> DeleteAsync(Timesheet timesheet, bool permanent = false);
}
