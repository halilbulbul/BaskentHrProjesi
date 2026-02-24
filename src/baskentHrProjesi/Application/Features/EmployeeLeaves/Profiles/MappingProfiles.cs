using Application.Features.EmployeeLeaves.Commands.Create;
using Application.Features.EmployeeLeaves.Commands.Delete;
using Application.Features.EmployeeLeaves.Commands.Update;
using Application.Features.EmployeeLeaves.Queries.GetById;
using Application.Features.EmployeeLeaves.Queries.GetList;
using AutoMapper;
using NArchitecture.Core.Application.Responses;
using Domain.Entities;
using NArchitecture.Core.Persistence.Paging;
using Application.Features.EmployeeLeaves.Queries.GetListByCurrentUser;

namespace Application.Features.EmployeeLeaves.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<CreateEmployeeLeaveCommand, EmployeeLeave>();
        CreateMap<EmployeeLeave, CreatedEmployeeLeaveResponse>();

        CreateMap<UpdateEmployeeLeaveCommand, EmployeeLeave>();
        CreateMap<EmployeeLeave, UpdatedEmployeeLeaveResponse>();

        CreateMap<DeleteEmployeeLeaveCommand, EmployeeLeave>();
        CreateMap<EmployeeLeave, DeletedEmployeeLeaveResponse>();

        CreateMap<EmployeeLeave, GetByIdEmployeeLeaveResponse>();

        CreateMap<EmployeeLeave, GetListEmployeeLeaveListItemDto>();
        CreateMap<EmployeeLeave, GetListByUserEmployeeLeaveListItemDto>();


        CreateMap<IPaginate<EmployeeLeave>, GetListResponse<GetListEmployeeLeaveListItemDto>>();
        CreateMap<IPaginate<EmployeeLeave>, GetListResponse<GetListByUserEmployeeLeaveListItemDto>>().ReverseMap();

        CreateMap<EmployeeLeave, GetListByUserEmployeeLeaveListItemDto>()
        .ForMember(
            dest => dest.LeaveTypeName,
            opt => opt.MapFrom(src => src.LeaveType.Name)
        )
        .ForMember(
            dest => dest.ApproverFullName,
            opt => opt.MapFrom(src =>
                src.ApproverEmployee != null
                    ? src.ApproverEmployee.FirstName + " " + src.ApproverEmployee.LastName
                    : string.Empty
            )
        );
    }
}