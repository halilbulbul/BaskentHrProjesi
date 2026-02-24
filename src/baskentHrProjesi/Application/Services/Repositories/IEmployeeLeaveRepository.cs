using Domain.Entities;
using NArchitecture.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IEmployeeLeaveRepository : IAsyncRepository<EmployeeLeave, int>, IRepository<EmployeeLeave, int>
{
}