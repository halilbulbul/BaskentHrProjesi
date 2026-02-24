using Application.Features.Auth.Constants;
using Application.Features.OperationClaims.Constants;
using Application.Features.UserOperationClaims.Constants;
using Application.Features.Users.Constants;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NArchitecture.Core.Security.Constants;
using Application.Features.Departments.Constants;
using Application.Features.Employees.Constants;
using Application.Features.EmployeeLeaves.Constants;
using Application.Features.Departments.Constants;
using Application.Features.Employees.Constants;
using Application.Features.EmployeeLeaves.Constants;
using Application.Features.LeaveTypes.Constants;
using Application.Features.Positions.Constants;
using Application.Features.TimeEntries.Constants;
using Application.Features.Timesheets.Constants;
using Application.Features.Timesheets.Constants;
using Application.Features.Shifts.Constants;
using Application.Features.Departments.Constants;
using Application.Features.Departments.Constants;
using Application.Features.Positions.Constants;
using Application.Features.Employees.Constants;



namespace Persistence.EntityConfigurations;

public class OperationClaimConfiguration : IEntityTypeConfiguration<OperationClaim>
{
    public void Configure(EntityTypeBuilder<OperationClaim> builder)
    {
        builder.ToTable("OperationClaims").HasKey(oc => oc.Id);

        builder.Property(oc => oc.Id).HasColumnName("Id").IsRequired();
        builder.Property(oc => oc.Name).HasColumnName("Name").IsRequired();
        builder.Property(oc => oc.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(oc => oc.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(oc => oc.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(oc => !oc.DeletedDate.HasValue);

        builder.HasData(_seeds);

        builder.HasBaseType((string)null!);
    }

    public static int AdminId => 1;
    private IEnumerable<OperationClaim> _seeds
    {
        get
        {
            yield return new() { Id = AdminId, Name = GeneralOperationClaims.Admin };

            IEnumerable<OperationClaim> featureOperationClaims = getFeatureOperationClaims(AdminId);
            foreach (OperationClaim claim in featureOperationClaims)
                yield return claim;
        }
    }

#pragma warning disable S1854 // Unused assignments should be removed
    private IEnumerable<OperationClaim> getFeatureOperationClaims(int initialId)
    {
        int lastId = initialId;
        List<OperationClaim> featureOperationClaims = new();

        #region Auth
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = AuthOperationClaims.Admin },
                new() { Id = ++lastId, Name = AuthOperationClaims.Read },
                new() { Id = ++lastId, Name = AuthOperationClaims.Write },
                new() { Id = ++lastId, Name = AuthOperationClaims.RevokeToken },
            ]
        );
        #endregion

        #region OperationClaims
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Admin },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Read },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Write },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Create },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Update },
                new() { Id = ++lastId, Name = OperationClaimsOperationClaims.Delete },
            ]
        );
        #endregion

        #region UserOperationClaims
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Admin },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Read },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Write },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Create },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Update },
                new() { Id = ++lastId, Name = UserOperationClaimsOperationClaims.Delete },
            ]
        );
        #endregion

        #region Users
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = UsersOperationClaims.Admin },
                new() { Id = ++lastId, Name = UsersOperationClaims.Read },
                new() { Id = ++lastId, Name = UsersOperationClaims.Write },
                new() { Id = ++lastId, Name = UsersOperationClaims.Create },
                new() { Id = ++lastId, Name = UsersOperationClaims.Update },
                new() { Id = ++lastId, Name = UsersOperationClaims.Delete },
            ]
        );
        #endregion

        
        #region Departments CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Admin },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Read },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Write },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Create },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Update },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Employees CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Admin },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Read },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Write },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Create },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Update },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region EmployeeLeaves CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Admin },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Read },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Write },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Create },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Update },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Departments CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Admin },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Read },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Write },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Create },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Update },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Employees CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Admin },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Read },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Write },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Create },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Update },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region EmployeeLeaves CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Admin },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Read },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Write },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Create },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Update },
                new() { Id = ++lastId, Name = EmployeeLeavesOperationClaims.Delete },
            ]
        );
        #endregion
        
              
        
        #region LeaveTypes CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = LeaveTypesOperationClaims.Admin },
                new() { Id = ++lastId, Name = LeaveTypesOperationClaims.Read },
                new() { Id = ++lastId, Name = LeaveTypesOperationClaims.Write },
                new() { Id = ++lastId, Name = LeaveTypesOperationClaims.Create },
                new() { Id = ++lastId, Name = LeaveTypesOperationClaims.Update },
                new() { Id = ++lastId, Name = LeaveTypesOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Positions CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = PositionsOperationClaims.Admin },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Read },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Write },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Create },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Update },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region TimeEntries CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = TimeEntriesOperationClaims.Admin },
                new() { Id = ++lastId, Name = TimeEntriesOperationClaims.Read },
                new() { Id = ++lastId, Name = TimeEntriesOperationClaims.Write },
                new() { Id = ++lastId, Name = TimeEntriesOperationClaims.Create },
                new() { Id = ++lastId, Name = TimeEntriesOperationClaims.Update },
                new() { Id = ++lastId, Name = TimeEntriesOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Timesheets CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Admin },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Read },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Write },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Create },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Update },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Timesheets CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Admin },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Read },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Write },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Create },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Update },
                new() { Id = ++lastId, Name = TimesheetsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Shifts CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = ShiftsOperationClaims.Admin },
                new() { Id = ++lastId, Name = ShiftsOperationClaims.Read },
                new() { Id = ++lastId, Name = ShiftsOperationClaims.Write },
                new() { Id = ++lastId, Name = ShiftsOperationClaims.Create },
                new() { Id = ++lastId, Name = ShiftsOperationClaims.Update },
                new() { Id = ++lastId, Name = ShiftsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Departments CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Admin },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Read },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Write },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Create },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Update },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Departments CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Admin },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Read },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Write },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Create },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Update },
                new() { Id = ++lastId, Name = DepartmentsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Positions CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = PositionsOperationClaims.Admin },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Read },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Write },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Create },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Update },
                new() { Id = ++lastId, Name = PositionsOperationClaims.Delete },
            ]
        );
        #endregion
        
        
        #region Employees CRUD
        featureOperationClaims.AddRange(
            [
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Admin },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Read },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Write },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Create },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Update },
                new() { Id = ++lastId, Name = EmployeesOperationClaims.Delete },
            ]
        );
        #endregion
        
        return featureOperationClaims;
    }
#pragma warning restore S1854 // Unused assignments should be removed
}
