using System;
using System.Collections.Generic;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities
{
    public class LeaveType : Entity<int>
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public bool IsPaid { get; set; }

        public virtual ICollection<EmployeeLeave> EmployeeLeaves { get; set; }

        public LeaveType()
        {
            EmployeeLeaves = new HashSet<EmployeeLeave>();
        }
    }
}
