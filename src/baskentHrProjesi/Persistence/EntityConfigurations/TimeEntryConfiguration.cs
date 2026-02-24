using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries").HasKey(te => te.Id);

        builder.Property(te => te.Id).HasColumnName("Id").IsRequired();
        builder.Property(te => te.EmployeeId).HasColumnName("EmployeeId").IsRequired();
        builder.Property(te => te.EventTime).HasColumnName("EventTime").IsRequired();
        builder.Property(te => te.Direction).HasColumnName("Direction").IsRequired();
        builder.Property(te => te.Source).HasColumnName("Source").IsRequired();
        builder.Property(te => te.DeviceId).HasColumnName("DeviceId").IsRequired();
        builder.Property(te => te.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(te => te.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(te => te.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(te => !te.DeletedDate.HasValue);
    }
}