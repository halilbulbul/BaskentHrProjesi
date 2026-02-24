using NArchitecture.Core.Application.Responses;
using System;

namespace Application.Features.UserOperationClaims.Queries.GetById;

public class GetByIdUserOperationClaimResponse : IResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int OperationClaimId { get; set; }

    public int? EmployeeId { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? EmployeeFirstName { get; set; }
    public string? EmployeeLastName { get; set; }
}
