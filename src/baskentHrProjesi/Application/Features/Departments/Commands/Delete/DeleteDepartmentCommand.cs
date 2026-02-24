using Application.Features.Departments.Constants;
using Application.Features.Departments.Constants;
using Application.Features.Departments.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using NArchitecture.Core.Application.Pipelines.Authorization;
using NArchitecture.Core.Application.Pipelines.Caching;
using NArchitecture.Core.Application.Pipelines.Logging;
using NArchitecture.Core.Application.Pipelines.Transaction;
using MediatR;
using static Application.Features.Departments.Constants.DepartmentsOperationClaims;

namespace Application.Features.Departments.Commands.Delete;

public class DeleteDepartmentCommand : IRequest<DeletedDepartmentResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public int Id { get; set; }

    public string[] Roles => [Admin, Write, DepartmentsOperationClaims.Delete];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetDepartments"];

    public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, DeletedDepartmentResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly DepartmentBusinessRules _departmentBusinessRules;

        public DeleteDepartmentCommandHandler(IMapper mapper, IDepartmentRepository departmentRepository,
                                         DepartmentBusinessRules departmentBusinessRules)
        {
            _mapper = mapper;
            _departmentRepository = departmentRepository;
            _departmentBusinessRules = departmentBusinessRules;
        }

        public async Task<DeletedDepartmentResponse> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            Department? department = await _departmentRepository.GetAsync(predicate: d => d.Id == request.Id, cancellationToken: cancellationToken);
            await _departmentBusinessRules.DepartmentShouldExistWhenSelected(department);

            await _departmentRepository.DeleteAsync(department!);

            DeletedDepartmentResponse response = _mapper.Map<DeletedDepartmentResponse>(department);
            return response;
        }
    }
}