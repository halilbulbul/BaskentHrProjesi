using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class TimesheetRepository : EfRepositoryBase<Timesheet, int, BaseDbContext>, ITimesheetRepository
{
    public TimesheetRepository(BaseDbContext context) : base(context)
    {
    }
}