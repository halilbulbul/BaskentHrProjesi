using System;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities
{
    public class Timesheet : Entity<int>
    {
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public DateTime WorkDate { get; set; }

        public int ShiftId { get; set; }
        public virtual Shift Shift { get; set; }

        public int PlannedMinutes { get; set; }
        public int ActualMinutes { get; set; }
        public int OvertimeMinutes { get; set; }
        public int MissingMinutes { get; set; }

        public int LateArrivalMinutes { get; set; }
        public int EarlyLeaveMinutes { get; set; }
        public int EarlyArrivalMinutes { get; set; }
        public int LateLeaveMinutes { get; set; }

        public string Status { get; set; }

        public int? EmployeeLeaveId { get; set; }
        public virtual EmployeeLeave EmployeeLeave { get; set; }
    }
}
