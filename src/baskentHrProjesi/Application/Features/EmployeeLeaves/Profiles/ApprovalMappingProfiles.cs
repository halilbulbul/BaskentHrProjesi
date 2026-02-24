using Application.Features.EmployeeLeaves.Commands.Approve;
using Application.Features.EmployeeLeaves.Commands.Reject;
using AutoMapper;
using Domain.Entities;

namespace Application.Features.EmployeeLeaves.Profiles;

public class ApprovalMappingProfiles : Profile
{
    public ApprovalMappingProfiles()
    {
        CreateMap<EmployeeLeave, ApprovedEmployeeLeaveResponse>();
        CreateMap<EmployeeLeave, RejectedEmployeeLeaveResponse>();
    }
}
