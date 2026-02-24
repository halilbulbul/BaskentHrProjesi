using System;
using System.Collections.Generic;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities
{
    public class Shift : Entity<int>
    {
        public string Name { get; set; }

        public TimeSpan ShiftStartTime { get; set; }
        public TimeSpan ShiftEndTime { get; set; }

        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }

        public int EarlyArrivalToleranceMinutes { get; set; }
        public int LateArrivalToleranceMinutes { get; set; }
        public int EarlyLeaveToleranceMinutes { get; set; }
        public int LateLeaveToleranceMinutes { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
        public virtual ICollection<Timesheet> Timesheets { get; set; }

        public Shift()
        {
            Employees = new HashSet<Employee>();
            Timesheets = new HashSet<Timesheet>();
        }
    }
}
