using NArchitecture.Core.Application.Dtos;
using System;

namespace Application.Features.UserOperationClaims.Queries.GetList;

public class GetListUserOperationClaimListItemDto : IDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public int OperationClaimId { get; set; }

    public int? EmployeeId { get; set; }
    public string? EmployeeNumber { get; set; }
    public string? EmployeeFirstName { get; set; }
    public string? EmployeeLastName { get; set; }
}
