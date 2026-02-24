using System;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities
{
    public class EmployeeLeave : Entity<int>
    {
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public int LeaveTypeId { get; set; }
        public virtual LeaveType? LeaveType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int TotalDays { get; set; }

        public string? ApprovalStatus { get; set; }

        public DateTime RequestDate { get; set; }

        public int? ApproverEmployeeId { get; set; }
        public virtual Employee? ApproverEmployee { get; set; }
    }
}
