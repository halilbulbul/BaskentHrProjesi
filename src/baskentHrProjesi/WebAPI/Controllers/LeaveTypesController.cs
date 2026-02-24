using Application.Features.EmployeeLeaves.Commands.Create;
using Application.Features.LeaveTypes.Commands.Create;
using Application.Features.LeaveTypes.Commands.Delete;
using Application.Features.LeaveTypes.Commands.Update;
using Application.Features.LeaveTypes.Queries.GetById;
using Application.Features.LeaveTypes.Queries.GetList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LeaveTypesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedLeaveTypeResponse>> Add([FromBody] CreateLeaveTypeCommand command)
    {
        command.IsPaid = false; // Default olarak izin tipleri ücretli olarak oluşturulur.
        CreatedLeaveTypeResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedLeaveTypeResponse>> Update([FromBody] UpdateLeaveTypeCommand command)
    {
        UpdatedLeaveTypeResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedLeaveTypeResponse>> Delete([FromRoute] int id)
    {
        DeleteLeaveTypeCommand command = new() { Id = id };

        DeletedLeaveTypeResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdLeaveTypeResponse>> GetById([FromRoute] int id)
    {
        GetByIdLeaveTypeQuery query = new() { Id = id };

        GetByIdLeaveTypeResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListLeaveTypeListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListLeaveTypeQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListLeaveTypeListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }


    [Authorize(Roles = "Personel")]
    [HttpPost("EmployeeLeaveRequest")]
    public async Task<ActionResult<CreatedEmployeeLeaveResponse>> EmployeeLeaveRequest(
        [FromBody] CreateEmployeeLeaveCommand command)
    {
        command.ApprovalStatus = "Beklemede";
        command.RequestDate = DateTime.UtcNow;

        CreatedEmployeeLeaveResponse response = await Mediator.Send(command);

        return Ok(response);
    }


}