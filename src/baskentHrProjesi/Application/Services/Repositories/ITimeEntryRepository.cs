using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface ITimeEntryRepository : IAsyncRepository<TimeEntry, int>, IRepository<TimeEntry, int>
{
}