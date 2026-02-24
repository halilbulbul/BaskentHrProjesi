using Application.Features.Timesheets.Commands.Create;
using Application.Features.Timesheets.Commands.Delete;
using Application.Features.Timesheets.Commands.Update;
using Application.Features.Timesheets.Queries.GetById;
using Application.Features.Timesheets.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Timesheets.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateTimesheetCommand, Timesheet>();
        CreateMap<Timesheet, CreatedTimesheetResponse>();

        CreateMap<UpdateTimesheetCommand, Timesheet>();
        CreateMap<Timesheet, UpdatedTimesheetResponse>();

        CreateMap<DeleteTimesheetCommand, Timesheet>();
        CreateMap<Timesheet, DeletedTimesheetResponse>();

        CreateMap<Timesheet, GetByIdTimesheetResponse>();

        CreateMap<Timesheet, GetListTimesheetListItemDto>();
        CreateMap<IPaginate<Timesheet>, GetListResponse<GetListTimesheetListItemDto>>();
    }
}