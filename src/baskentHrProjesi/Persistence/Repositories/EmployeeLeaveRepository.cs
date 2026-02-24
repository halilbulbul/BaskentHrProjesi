using Application.Services.Repositories;
using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class EmployeeLeaveRepository : EfRepositoryBase<EmployeeLeave, int, BaseDbContext>, IEmployeeLeaveRepository
{
    public EmployeeLeaveRepository(BaseDbContext context) : base(context)
    {
    }
}