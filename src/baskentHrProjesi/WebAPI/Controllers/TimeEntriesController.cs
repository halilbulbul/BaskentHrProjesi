using Application.Features.TimeEntries.Commands.Create;
using Application.Features.TimeEntries.Commands.Delete;
using Application.Features.TimeEntries.Commands.Update;
using Application.Features.TimeEntries.Queries.GetById;
using Application.Features.TimeEntries.Queries.GetList;
using Microsoft.AspNetCore.Mvc;
using NArchitecture.Core.Application.Requests;
using NArchitecture.Core.Application.Responses;
using NArchitecture.Core.Persistence.Dynamic;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TimeEntriesController : BaseController
{
    [HttpPost]
    public async Task<ActionResult<CreatedTimeEntryResponse>> Add([FromBody] CreateTimeEntryCommand command)
    {
        CreatedTimeEntryResponse response = await Mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { response.Id }, response);
    }

    [HttpPut]
    public async Task<ActionResult<UpdatedTimeEntryResponse>> Update([FromBody] UpdateTimeEntryCommand command)
    {
        UpdatedTimeEntryResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<DeletedTimeEntryResponse>> Delete([FromRoute] int id)
    {
        DeleteTimeEntryCommand command = new() { Id = id };

        DeletedTimeEntryResponse response = await Mediator.Send(command);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetByIdTimeEntryResponse>> GetById([FromRoute] int id)
    {
        GetByIdTimeEntryQuery query = new() { Id = id };

        GetByIdTimeEntryResponse response = await Mediator.Send(query);

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<GetListResponse<GetListTimeEntryListItemDto>>> GetList(
      [FromQuery] PageRequest pageRequest,
      [FromQuery] int? employeeId,
      [FromQuery] int? year,
      [FromQuery] int? month)
    {
        GetListTimeEntryQuery query = new()
        {
            PageRequest = pageRequest,
            EmployeeId = employeeId,
            Year = year,
            Month = month,
            Dynamic = new DynamicQuery() // boş dynamic ile GetListByDynamicAsync çalışsın
        };

        GetListResponse<GetListTimeEntryListItemDto> response = await Mediator.Send(query);

        return Ok(response);
    }
}