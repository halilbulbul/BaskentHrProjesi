using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface ILeaveTypeRepository : IAsyncRepository<LeaveType, int>, IRepository<LeaveType, int>
{
}