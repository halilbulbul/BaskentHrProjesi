using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class PositionRepository : EfRepositoryBase<Position, int, BaseDbContext>, IPositionRepository
{
    public PositionRepository(BaseDbContext context) : base(context)
    {
    }
}