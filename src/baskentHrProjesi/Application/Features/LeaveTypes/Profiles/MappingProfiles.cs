using Application.Features.LeaveTypes.Commands.Create;
using Application.Features.LeaveTypes.Commands.Delete;
using Application.Features.LeaveTypes.Commands.Update;
using Application.Features.LeaveTypes.Queries.GetById;
using Application.Features.LeaveTypes.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;

namespace Application.Features.LeaveTypes.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateLeaveTypeCommand, LeaveType>();
        CreateMap<LeaveType, CreatedLeaveTypeResponse>();

        CreateMap<UpdateLeaveTypeCommand, LeaveType>();
        CreateMap<LeaveType, UpdatedLeaveTypeResponse>();

        CreateMap<DeleteLeaveTypeCommand, LeaveType>();
        CreateMap<LeaveType, DeletedLeaveTypeResponse>();

        CreateMap<LeaveType, GetByIdLeaveTypeResponse>();

        CreateMap<LeaveType, GetListLeaveTypeListItemDto>();
        CreateMap<IPaginate<LeaveType>, GetListResponse<GetListLeaveTypeListItemDto>>();
    }
}