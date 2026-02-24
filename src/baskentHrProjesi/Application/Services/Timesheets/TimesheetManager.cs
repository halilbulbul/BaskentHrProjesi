using Application.Features.Timesheets.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.Timesheets;

public class TimesheetManager : ITimesheetService
{
    private readonly ITimesheetRepository _timesheetRepository;
    private readonly TimesheetBusinessRules _timesheetBusinessRules;

    public TimesheetManager(ITimesheetRepository timesheetRepository, TimesheetBusinessRules timesheetBusinessRules)
    {
        _timesheetRepository = timesheetRepository;
        _timesheetBusinessRules = timesheetBusinessRules;
    }

    public async Task<Timesheet?> GetAsync(
        Expression<Func<Timesheet, bool>> predicate,
        Func<IQueryable<Timesheet>, IIncludableQueryable<Timesheet, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        Timesheet? timesheet = await _timesheetRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return timesheet;
    }

    public async Task<IPaginate<Timesheet>?> GetListAsync(
        Expression<Func<Timesheet, bool>>? predicate = null,
        Func<IQueryable<Timesheet>, IOrderedQueryable<Timesheet>>? orderBy = null,
        Func<IQueryable<Timesheet>, IIncludableQueryable<Timesheet, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<Timesheet> timesheetList = await _timesheetRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return timesheetList;
    }

    public async Task<Timesheet> AddAsync(Timesheet timesheet)
    {
        Timesheet addedTimesheet = await _timesheetRepository.AddAsync(timesheet);

        return addedTimesheet;
    }

    public async Task<Timesheet> UpdateAsync(Timesheet timesheet)
    {
        Timesheet updatedTimesheet = await _timesheetRepository.UpdateAsync(timesheet);

        return updatedTimesheet;
    }

    public async Task<Timesheet> DeleteAsync(Timesheet timesheet, bool permanent = false)
    {
        Timesheet deletedTimesheet = await _timesheetRepository.DeleteAsync(timesheet);

        return deletedTimesheet;
    }
}
