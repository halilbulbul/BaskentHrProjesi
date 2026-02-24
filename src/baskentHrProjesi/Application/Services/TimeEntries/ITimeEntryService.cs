using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.TimeEntries;

public interface ITimeEntryService
{
    Task<TimeEntry?> GetAsync(
        Expression<Func<TimeEntry, bool>> predicate,
        Func<IQueryable<TimeEntry>, IIncludableQueryable<TimeEntry, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<TimeEntry>?> GetListAsync(
        Expression<Func<TimeEntry, bool>>? predicate = null,
        Func<IQueryable<TimeEntry>, IOrderedQueryable<TimeEntry>>? orderBy = null,
        Func<IQueryable<TimeEntry>, IIncludableQueryable<TimeEntry, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<TimeEntry> AddAsync(TimeEntry timeEntry);
    Task<TimeEntry> UpdateAsync(TimeEntry timeEntry);
    Task<TimeEntry> DeleteAsync(TimeEntry timeEntry, bool permanent = false);
}
