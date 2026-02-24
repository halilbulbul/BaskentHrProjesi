using Application.Features.Shifts.Commands.Create;
using Application.Features.Shifts.Commands.Delete;
using Application.Features.Shifts.Commands.Update;
using Application.Features.Shifts.Queries.GetById;
using Application.Features.Shifts.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.Shifts.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateShiftCommand, Shift>();
        CreateMap<Shift, CreatedShiftResponse>();

        CreateMap<UpdateShiftCommand, Shift>();
        CreateMap<Shift, UpdatedShiftResponse>();

        CreateMap<DeleteShiftCommand, Shift>();
        CreateMap<Shift, DeletedShiftResponse>();

        CreateMap<Shift, GetByIdShiftResponse>();

        CreateMap<Shift, GetListShiftListItemDto>();
        CreateMap<IPaginate<Shift>, GetListResponse<GetListShiftListItemDto>>();
    }
}