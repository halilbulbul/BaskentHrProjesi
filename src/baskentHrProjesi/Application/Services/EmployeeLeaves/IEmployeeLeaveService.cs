using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.EmployeeLeaves;

public interface IEmployeeLeaveService
{
    Task<EmployeeLeave?> GetAsync(
        Expression<Func<EmployeeLeave, bool>> predicate,
        Func<IQueryable<EmployeeLeave>, IIncludableQueryable<EmployeeLeave, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<IPaginate<EmployeeLeave>?> GetListAsync(
        Expression<Func<EmployeeLeave, bool>>? predicate = null,
        Func<IQueryable<EmployeeLeave>, IOrderedQueryable<EmployeeLeave>>? orderBy = null,
        Func<IQueryable<EmployeeLeave>, IIncludableQueryable<EmployeeLeave, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    );
    Task<EmployeeLeave> AddAsync(EmployeeLeave employeeLeave);
    Task<EmployeeLeave> UpdateAsync(EmployeeLeave employeeLeave);
    Task<EmployeeLeave> DeleteAsync(EmployeeLeave employeeLeave, bool permanent = false);
}
