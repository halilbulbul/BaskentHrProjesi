using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class LeaveTypeRepository : EfRepositoryBase<LeaveType, int, BaseDbContext>, ILeaveTypeRepository
{
    public LeaveTypeRepository(BaseDbContext context) : base(context)
    {
    }
}