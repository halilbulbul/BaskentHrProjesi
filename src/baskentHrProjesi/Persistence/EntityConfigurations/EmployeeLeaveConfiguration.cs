using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class EmployeeLeaveConfiguration : IEntityTypeConfiguration<EmployeeLeave>
{
    public void Configure(EntityTypeBuilder<EmployeeLeave> builder)
    {
        builder.ToTable("EmployeeLeaves").HasKey(el => el.Id);

        builder.Property(el => el.Id).HasColumnName("Id").IsRequired();
        builder.Property(el => el.EmployeeId).HasColumnName("EmployeeId").IsRequired();
        builder.Property(el => el.LeaveTypeId).HasColumnName("LeaveTypeId").IsRequired();
        builder.Property(el => el.StartDate).HasColumnName("StartDate").IsRequired();
        builder.Property(el => el.EndDate).HasColumnName("EndDate").IsRequired();
        builder.Property(el => el.TotalDays).HasColumnName("TotalDays").IsRequired();
        builder.Property(el => el.ApprovalStatus).HasColumnName("ApprovalStatus").IsRequired();
        builder.Property(el => el.RequestDate).HasColumnName("RequestDate").IsRequired();
        builder.Property(el => el.ApproverEmployeeId).HasColumnName("ApproverEmployeeId");
        builder.Property(el => el.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(el => el.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(el => el.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(el => !el.DeletedDate.HasValue);
    }
}