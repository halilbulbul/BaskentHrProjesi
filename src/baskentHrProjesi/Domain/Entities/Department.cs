using System;
using System.Collections.Generic;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities
{
    public class Department : Entity<int>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }

        public Department()
        {
            Employees = new HashSet<Employee>();
        }
    }
}
