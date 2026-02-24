using Application.Features.EmployeeLeaves.Commands.Approve;
using Application.Features.EmployeeLeaves.Commands.Create;
using Application.Features.EmployeeLeaves.Commands.Delete;
using Application.Features.EmployeeLeaves.Commands.Reject;
using Application.Features.EmployeeLeaves.Commands.Update;
using Application.Features.EmployeeLeaves.Queries.GetById;
using Application.Features.EmployeeLeaves.Queries.GetList;
using Application.Features.EmployeeLeaves.Queries.GetListByCurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using System.Security.Claims;


namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeLeavesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedEmployeeLeaveResponse>> Add([FromBody] CreateEmployeeLeaveCommand command)
    {
        CreatedEmployeeLeaveResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedEmployeeLeaveResponse>> Update([FromBody] UpdateEmployeeLeaveCommand command)
    {
        UpdatedEmployeeLeaveResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedEmployeeLeaveResponse>> Delete([FromRoute] int id)
    {
        DeleteEmployeeLeaveCommand command = new() { Id = id };

        DeletedEmployeeLeaveResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdEmployeeLeaveResponse>> GetById([FromRoute] int id)
    {
        GetByIdEmployeeLeaveQuery query = new() { Id = id };

        GetByIdEmployeeLeaveResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListEmployeeLeaveListItemDto>>> GetList(
        [FromQuery] PageRequest pageRequest)
    {
        GetListEmployeeLeaveQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListEmployeeLeaveListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }


    [HttpGet("GetMyLeaves")]
    public async Task<ActionResult<GetListResponse<GetListByUserEmployeeLeaveListItemDto>>> GetMyLeaves([FromQuery] PageRequest pageRequest)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized();

        Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            return Unauthorized();

        GetListByCurrentUserEmployeeLeavesQuery query = new()
        {
            PageRequest = pageRequest,
            UserId = userId
        };

        GetListResponse<GetListByUserEmployeeLeaveListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("EmployeeLeaveRequest")]
    public async Task<ActionResult<CreatedEmployeeLeaveResponse>> EmployeeLeaveRequest(
    [FromBody] CreateEmployeeLeaveCommand command)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized();

        Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            return Unauthorized();

        command.EmployeeUserId = userId;
        command.ApprovalStatus ??= "Beklemede";

        CreatedEmployeeLeaveResponse response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("{id}/approve")]
    public async Task<ActionResult<ApprovedEmployeeLeaveResponse>> Approve([FromRoute] int id)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized();

        Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            return Unauthorized();

        ApproveEmployeeLeaveCommand command = new()
        {
            Id = id,
            ApproverUserId = userId
        };

        ApprovedEmployeeLeaveResponse response = await Mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("{id}/reject")]
    public async Task<ActionResult<RejectedEmployeeLeaveResponse>> Reject([FromRoute] int id)
    {
        if (User.Identity == null || !User.Identity.IsAuthenticated)
            return Unauthorized();

        Claim? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            return Unauthorized();

        RejectEmployeeLeaveCommand command = new()
        {
            Id = id,
            ApproverUserId = userId
        };

        RejectedEmployeeLeaveResponse response = await Mediator.Send(command);
        return Ok(response);
    }


}
