using NArchitecture.Core.Application.Responses;
using System;

namespace Application.Features.Employees.Commands.Update;

public class UpdatedEmployeeResponse : IResponse
{
    public int Id { get; set; }
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
    public int? PositionId { get; set; }
    public int? ShiftId { get; set; }
    public Guid? UserId { get; set; }
}
