using System;
using System.Collections.Generic;
using NArchitecture.Core.Persistence.Repositories;

namespace Domain.Entities
{
    public class Employee : Entity<int>
    {
        public string EmployeeNumber { get; set; }

        public string IdentityNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string MaritalStatus { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public string AddressLine { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string PostalCode { get; set; }

        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }

        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public int? PositionId { get; set; }
        public virtual Position Position { get; set; }

        public int? ShiftId { get; set; }
        public virtual Shift Shift { get; set; }

        public virtual ICollection<Timesheet> Timesheets { get; set; }

        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}
