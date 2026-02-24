using System.Reflection;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Contexts;

public class BaseDbContext : DbContext
{
    protected IConfiguration Configuration { get; set; }

    public DbSet<EmailAuthenticator> EmailAuthenticators { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<OtpAuthenticator> OtpAuthenticators { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeLeave> EmployeeLeaves { get; set; }
    public DbSet<LeaveType> LeaveTypes { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<TimeEntry> TimeEntries { get; set; }
    public DbSet<Timesheet> Timesheets { get; set; }
    public DbSet<Shift> Shifts { get; set; }

    public BaseDbContext(DbContextOptions<BaseDbContext> options, IConfiguration configuration)
        : base(options)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var cs = Configuration.GetConnectionString("BaseDb");
            optionsBuilder.UseSqlServer(cs);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<Employee>(b =>
        {
            b.HasOne(e => e.User)
             .WithOne()
             .HasForeignKey<Employee>(e => e.UserId)
             .OnDelete(DeleteBehavior.Restrict);
        });

    }
}
