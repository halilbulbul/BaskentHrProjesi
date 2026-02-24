using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees").HasKey(e => e.Id);

        builder.Property(e => e.Id).HasColumnName("Id").IsRequired();
        builder.Property(e => e.EmployeeNumber).HasColumnName("EmployeeNumber").IsRequired();
        builder.Property(e => e.IdentityNo).HasColumnName("IdentityNo").IsRequired();
        builder.Property(e => e.FirstName).HasColumnName("FirstName").IsRequired();
        builder.Property(e => e.LastName).HasColumnName("LastName").IsRequired();
        builder.Property(e => e.BirthDate).HasColumnName("BirthDate");
        builder.Property(e => e.Gender).HasColumnName("Gender").IsRequired();
        builder.Property(e => e.MaritalStatus).HasColumnName("MaritalStatus").IsRequired();
        builder.Property(e => e.PhoneNumber).HasColumnName("PhoneNumber").IsRequired();
        builder.Property(e => e.Email).HasColumnName("Email").IsRequired();
        builder.Property(e => e.AddressLine).HasColumnName("AddressLine").IsRequired();
        builder.Property(e => e.City).HasColumnName("City").IsRequired();
        builder.Property(e => e.District).HasColumnName("District").IsRequired();
        builder.Property(e => e.PostalCode).HasColumnName("PostalCode").IsRequired();
        builder.Property(e => e.HireDate).HasColumnName("HireDate").IsRequired();
        builder.Property(e => e.TerminationDate).HasColumnName("TerminationDate");
        builder.Property(e => e.DepartmentId).HasColumnName("DepartmentId").IsRequired();
        builder.Property(e => e.PositionId).HasColumnName("PositionId");
        builder.Property(e => e.ShiftId).HasColumnName("ShiftId");
        builder.Property(e => e.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(e => e.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(e => e.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(e => !e.DeletedDate.HasValue);
    }
}