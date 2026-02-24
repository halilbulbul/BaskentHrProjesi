using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IPositionRepository : IAsyncRepository<Position, int>, IRepository<Position, int>
{
}