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

namespace Application.Features.Departments.Commands.Create;

public class CreateDepartmentCommand : IRequest<CreatedDepartmentResponse>, ISecuredRequest, ICacheRemoverRequest, ILoggableRequest, ITransactionalRequest
{
    public required string Code { get; set; }
    public required string Name { get; set; }

    public string[] Roles => [Admin, Write, DepartmentsOperationClaims.Create];

    public bool BypassCache { get; }
    public string? CacheKey { get; }
    public string[]? CacheGroupKey => ["GetDepartments"];

    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, CreatedDepartmentResponse>
    {
        private readonly IMapper _mapper;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly DepartmentBusinessRules _departmentBusinessRules;

        public CreateDepartmentCommandHandler(IMapper mapper, IDepartmentRepository departmentRepository,
                                         DepartmentBusinessRules departmentBusinessRules)
        {
            _mapper = mapper;
            _departmentRepository = departmentRepository;
            _departmentBusinessRules = departmentBusinessRules;
        }

        public async Task<CreatedDepartmentResponse> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            Department department = _mapper.Map<Department>(request);

            await _departmentRepository.AddAsync(department);

            CreatedDepartmentResponse response = _mapper.Map<CreatedDepartmentResponse>(department);
            return response;
        }
    }
}