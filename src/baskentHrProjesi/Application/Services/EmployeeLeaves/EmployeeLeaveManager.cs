using Application.Features.EmployeeLeaves.Rules;
using Application.Services.Repositories;
using NArchitecture.Core.Persistence.Paging;
using Domain.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Services.EmployeeLeaves;

public class EmployeeLeaveManager : IEmployeeLeaveService
{
    private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
    private readonly EmployeeLeaveBusinessRules _employeeLeaveBusinessRules;

    public EmployeeLeaveManager(IEmployeeLeaveRepository employeeLeaveRepository, EmployeeLeaveBusinessRules employeeLeaveBusinessRules)
    {
        _employeeLeaveRepository = employeeLeaveRepository;
        _employeeLeaveBusinessRules = employeeLeaveBusinessRules;
    }

    public async Task<EmployeeLeave?> GetAsync(
        Expression<Func<EmployeeLeave, bool>> predicate,
        Func<IQueryable<EmployeeLeave>, IIncludableQueryable<EmployeeLeave, object>>? include = null,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        EmployeeLeave? employeeLeave = await _employeeLeaveRepository.GetAsync(predicate, include, withDeleted, enableTracking, cancellationToken);
        return employeeLeave;
    }

    public async Task<IPaginate<EmployeeLeave>?> GetListAsync(
        Expression<Func<EmployeeLeave, bool>>? predicate = null,
        Func<IQueryable<EmployeeLeave>, IOrderedQueryable<EmployeeLeave>>? orderBy = null,
        Func<IQueryable<EmployeeLeave>, IIncludableQueryable<EmployeeLeave, object>>? include = null,
        int index = 0,
        int size = 10,
        bool withDeleted = false,
        bool enableTracking = true,
        CancellationToken cancellationToken = default
    )
    {
        IPaginate<EmployeeLeave> employeeLeaveList = await _employeeLeaveRepository.GetListAsync(
            predicate,
            orderBy,
            include,
            index,
            size,
            withDeleted,
            enableTracking,
            cancellationToken
        );
        return employeeLeaveList;
    }

    public async Task<EmployeeLeave> AddAsync(EmployeeLeave employeeLeave)
    {
        EmployeeLeave addedEmployeeLeave = await _employeeLeaveRepository.AddAsync(employeeLeave);

        return addedEmployeeLeave;
    }

    public async Task<EmployeeLeave> UpdateAsync(EmployeeLeave employeeLeave)
    {
        EmployeeLeave updatedEmployeeLeave = await _employeeLeaveRepository.UpdateAsync(employeeLeave);

        return updatedEmployeeLeave;
    }

    public async Task<EmployeeLeave> DeleteAsync(EmployeeLeave employeeLeave, bool permanent = false)
    {
        EmployeeLeave deletedEmployeeLeave = await _employeeLeaveRepository.DeleteAsync(employeeLeave);

        return deletedEmployeeLeave;
    }
}
