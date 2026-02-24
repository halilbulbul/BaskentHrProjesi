using System;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities
{
    public class TimeEntry : Entity<int>
    {
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public DateTime EventTime { get; set; }

        public string Direction { get; set; }

        public string Source { get; set; }

        public string DeviceId { get; set; }
    }
}
