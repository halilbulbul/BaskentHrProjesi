using Application.Features.TimeEntries.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.TimeEntries;

public class TimeEntryManager : ITimeEntryService
{
    private readonly ITimeEntryRepository _timeEntryRepository;
    private readonly TimeEntryBusinessRules _timeEntryBusinessRules;

    public TimeEntryManager(ITimeEntryRepository timeEntryRepository, TimeEntryBusinessRules timeEntryBusinessRules)
    {
        _timeEntryRepository = timeEntryRepository;
        _timeEntryBusinessRules = timeEntryBusinessRules;
    }

    public async Task<TimeEntry?> GetAsync(
        Expression<Func<TimeEntry, bool>> predicate,
        Func<IQueryable<TimeEntry>, IIncludableQueryable<TimeEntry, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        TimeEntry? timeEntry = await _timeEntryRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return timeEntry;
    }

    public async Task<IPaginate<TimeEntry>?> GetListAsync(
        Expression<Func<TimeEntry, bool>>? predicate = null,
        Func<IQueryable<TimeEntry>, IOrderedQueryable<TimeEntry>>? orderBy = null,
        Func<IQueryable<TimeEntry>, IIncludableQueryable<TimeEntry, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<TimeEntry> timeEntryList = await _timeEntryRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return timeEntryList;
    }

    public async Task<TimeEntry> AddAsync(TimeEntry timeEntry)
    {
        TimeEntry addedTimeEntry = await _timeEntryRepository.AddAsync(timeEntry);

        return addedTimeEntry;
    }

    public async Task<TimeEntry> UpdateAsync(TimeEntry timeEntry)
    {
        TimeEntry updatedTimeEntry = await _timeEntryRepository.UpdateAsync(timeEntry);

        return updatedTimeEntry;
    }

    public async Task<TimeEntry> DeleteAsync(TimeEntry timeEntry, bool permanent = false)
    {
        TimeEntry deletedTimeEntry = await _timeEntryRepository.DeleteAsync(timeEntry);

        return deletedTimeEntry;
    }
}
