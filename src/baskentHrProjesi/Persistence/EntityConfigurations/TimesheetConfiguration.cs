using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class TimesheetConfiguration : IEntityTypeConfiguration<Timesheet>
{
    public void Configure(EntityTypeBuilder<Timesheet> builder)
    {
        builder.ToTable("Timesheets").HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("Id").IsRequired();
        builder.Property(t => t.EmployeeId).HasColumnName("EmployeeId").IsRequired();
        builder.Property(t => t.WorkDate).HasColumnName("WorkDate").IsRequired();
        builder.Property(t => t.ShiftId).HasColumnName("ShiftId").IsRequired();
        builder.Property(t => t.PlannedMinutes).HasColumnName("PlannedMinutes").IsRequired();
        builder.Property(t => t.ActualMinutes).HasColumnName("ActualMinutes").IsRequired();
        builder.Property(t => t.OvertimeMinutes).HasColumnName("OvertimeMinutes").IsRequired();
        builder.Property(t => t.MissingMinutes).HasColumnName("MissingMinutes").IsRequired();
        builder.Property(t => t.LateArrivalMinutes).HasColumnName("LateArrivalMinutes").IsRequired();
        builder.Property(t => t.EarlyLeaveMinutes).HasColumnName("EarlyLeaveMinutes").IsRequired();
        builder.Property(t => t.EarlyArrivalMinutes).HasColumnName("EarlyArrivalMinutes").IsRequired();
        builder.Property(t => t.LateLeaveMinutes).HasColumnName("LateLeaveMinutes").IsRequired();
        builder.Property(t => t.Status).HasColumnName("Status").IsRequired();
        builder.Property(t => t.EmployeeLeaveId).HasColumnName("EmployeeLeaveId");
        builder.Property(t => t.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(t => t.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(t => t.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(t => !t.DeletedDate.HasValue);
    }
}