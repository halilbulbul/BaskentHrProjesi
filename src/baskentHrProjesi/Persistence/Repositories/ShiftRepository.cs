using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class ShiftRepository : EfRepositoryBase<Shift, int, BaseDbContext>, IShiftRepository
{
    public ShiftRepository(BaseDbContext context) : base(context)
    {
    }
}