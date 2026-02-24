using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IShiftRepository : IAsyncRepository<Shift, int>, IRepository<Shift, int>
{
}