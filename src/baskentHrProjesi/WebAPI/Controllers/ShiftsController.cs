using Application.Features.Shifts.Commands.Create;
using Application.Features.Shifts.Commands.Delete;
using Application.Features.Shifts.Commands.Update;
using Application.Features.Shifts.Queries.GetById;
using Application.Features.Shifts.Queries.GetList;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ShiftsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedShiftResponse>> Add([FromBody] CreateShiftCommand command)
    {
        CreatedShiftResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedShiftResponse>> Update([FromBody] UpdateShiftCommand command)
    {
        UpdatedShiftResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedShiftResponse>> Delete([FromRoute] int id)
    {
        DeleteShiftCommand command = new() { Id = id };

        DeletedShiftResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdShiftResponse>> GetById([FromRoute] int id)
    {
        GetByIdShiftQuery query = new() { Id = id };

        GetByIdShiftResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListShiftListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListShiftQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListShiftListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}