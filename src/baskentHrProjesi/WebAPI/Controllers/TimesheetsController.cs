using Application.Features.Timesheets.Calculate;
using Application.Features.Timesheets.Commands.Create;
using Application.Features.Timesheets.Commands.Delete;
using Application.Features.Timesheets.Commands.Update;
using Application.Features.Timesheets.Queries.GetById;
using Application.Features.Timesheets.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimesheetsController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedTimesheetResponse>> Add([FromBody] CreateTimesheetCommand command)
    {
        CreatedTimesheetResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedTimesheetResponse>> Update([FromBody] UpdateTimesheetCommand command)
    {
        UpdatedTimesheetResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedTimesheetResponse>> Delete([FromRoute] int id)
    {
        DeleteTimesheetCommand command = new() { Id = id };

        DeletedTimesheetResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdTimesheetResponse>> GetById([FromRoute] int id)
    {
        GetByIdTimesheetQuery query = new() { Id = id };

        GetByIdTimesheetResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListTimesheetListItemDto>>> GetList([FromQuery] PageRequest pageRequest)
    {
        GetListTimesheetQuery query = new() { PageRequest = pageRequest };

        GetListResponse<GetListTimesheetListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpPost("CalculateMonthly")]
    public async Task<ActionResult<IList<GetListTimesheetListItemDto>>> CalculateMonthly(
        [FromBody] CalculateMonthlyTimesheetQuery query)
    {
        IList<GetListTimesheetListItemDto> result = await Mediator.Send(query);
        return Ok(result);
    }


}