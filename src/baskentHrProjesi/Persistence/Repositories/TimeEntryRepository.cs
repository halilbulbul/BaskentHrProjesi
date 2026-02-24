using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class TimeEntryRepository : EfRepositoryBase<TimeEntry, int, BaseDbContext>, ITimeEntryRepository
{
    public TimeEntryRepository(BaseDbContext context) : base(context)
    {
    }
}