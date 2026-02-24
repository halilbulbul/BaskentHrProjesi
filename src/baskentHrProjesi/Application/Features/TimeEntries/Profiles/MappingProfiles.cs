using Application.Features.TimeEntries.Commands.Create;
using Application.Features.TimeEntries.Commands.Delete;
using Application.Features.TimeEntries.Commands.Update;
using Application.Features.TimeEntries.Queries.GetById;
using Application.Features.TimeEntries.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.TimeEntries.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateTimeEntryCommand, TimeEntry>();
        CreateMap<TimeEntry, CreatedTimeEntryResponse>();

        CreateMap<UpdateTimeEntryCommand, TimeEntry>();
        CreateMap<TimeEntry, UpdatedTimeEntryResponse>();

        CreateMap<DeleteTimeEntryCommand, TimeEntry>();
        CreateMap<TimeEntry, DeletedTimeEntryResponse>();

        CreateMap<TimeEntry, GetByIdTimeEntryResponse>();

        CreateMap<TimeEntry, GetListTimeEntryListItemDto>()
            .ForMember(
                dest => dest.EmployeeFullName,
                opt => opt.MapFrom(src =>
                    src.Employee != null
                        ? src.Employee.FirstName + " " + src.Employee.LastName
                        : string.Empty
                )
            );

        CreateMap<IPaginate<TimeEntry>, GetListResponse<GetListTimeEntryListItemDto>>();
    }
}
