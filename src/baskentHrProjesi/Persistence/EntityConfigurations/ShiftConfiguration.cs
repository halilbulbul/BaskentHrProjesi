using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts").HasKey(trs => trs.Id);

        builder.Property(trs => trs.Id).HasColumnName("Id").IsRequired();
        builder.Property(trs => trs.Name).HasColumnName("Name").IsRequired();
        builder.Property(trs => trs.ShiftStartTime).HasColumnName("ShiftStartTime").IsRequired();
        builder.Property(trs => trs.ShiftEndTime).HasColumnName("ShiftEndTime").IsRequired();
        builder.Property(trs => trs.BreakStartTime).HasColumnName("BreakStartTime");
        builder.Property(trs => trs.BreakEndTime).HasColumnName("BreakEndTime");
        builder.Property(trs => trs.EarlyArrivalToleranceMinutes).HasColumnName("EarlyArrivalToleranceMinutes").IsRequired();
        builder.Property(trs => trs.LateArrivalToleranceMinutes).HasColumnName("LateArrivalToleranceMinutes").IsRequired();
        builder.Property(trs => trs.EarlyLeaveToleranceMinutes).HasColumnName("EarlyLeaveToleranceMinutes").IsRequired();
        builder.Property(trs => trs.LateLeaveToleranceMinutes).HasColumnName("LateLeaveToleranceMinutes").IsRequired();
        builder.Property(trs => trs.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(trs => trs.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(trs => trs.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(trs => !trs.DeletedDate.HasValue);
    }
}