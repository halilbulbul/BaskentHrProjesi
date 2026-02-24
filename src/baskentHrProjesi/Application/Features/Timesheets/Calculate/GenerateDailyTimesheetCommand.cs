using Application.Features.EmployeeLeaves.Queries.GetList;
using Application.Features.LeaveTypes.Queries.GetList;
using Application.Features.Shifts.Queries.GetById;
using Application.Features.TimeEntries.Queries.GetList;
using Application.Features.Timesheets.Commands.Create;
using Application.Services.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Timesheets.Calculate
{
    public class GenerateDailyTimesheetCommand : IRequest<CreatedTimesheetResponse>
    {
        public int EmployeeId { get; set; }
        public DateTime WorkDate { get; set; }
    }

    public class GenerateDailyTimesheetCommandHandler
        : IRequestHandler<GenerateDailyTimesheetCommand, CreatedTimesheetResponse>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IShiftRepository _ShiftRepository;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GenerateDailyTimesheetCommandHandler(
            IEmployeeRepository employeeRepository,
            IShiftRepository ShiftRepository,
            ITimeEntryRepository timeEntryRepository,
            IEmployeeLeaveRepository employeeLeaveRepository,
            ILeaveTypeRepository leaveTypeRepository,
            IMediator mediator,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _ShiftRepository = ShiftRepository;
            _timeEntryRepository = timeEntryRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<CreatedTimesheetResponse> Handle(
            GenerateDailyTimesheetCommand request,
            CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetAsync(
                e => e.Id == request.EmployeeId,
                cancellationToken: cancellationToken);

            if (employee == null)
                throw new Exception("Personel bulunamadı.");

            if (employee.ShiftId == null)
                throw new Exception("Personelin tanımlı puantaj kural seti yok.");

            var ruleSetEntity = await _ShiftRepository.GetAsync(
                r => r.Id == employee.ShiftId,
                cancellationToken: cancellationToken);

            if (ruleSetEntity == null)
                throw new Exception("Puantaj kural seti bulunamadı.");

            var ruleSetDto = _mapper.Map<GetByIdShiftResponse>(ruleSetEntity);

            var day = request.WorkDate.Date;
            var nextDay = day.AddDays(1);

            var timeEntriesPage = await _timeEntryRepository.GetListAsync(
                predicate: t => t.EmployeeId == request.EmployeeId &&
                                t.EventTime >= day &&
                                t.EventTime < nextDay,
                cancellationToken: cancellationToken);

            var timeEntryDtos = _mapper.Map<GetListTimeEntryListItemDto[]>(timeEntriesPage.Items);

            var employeeLeavesPage = await _employeeLeaveRepository.GetListAsync(
                predicate: l => l.EmployeeId == request.EmployeeId &&
                                l.EndDate >= day &&
                                l.StartDate <= nextDay,
                cancellationToken: cancellationToken);

            var employeeLeaveDtos = _mapper.Map<GetListEmployeeLeaveListItemDto[]>(employeeLeavesPage.Items);

            var leaveTypeIds = employeeLeaveDtos.Select(l => l.LeaveTypeId).Distinct().ToArray();

            var leaveTypesPage = leaveTypeIds.Any()
                ? await _leaveTypeRepository.GetListAsync(
                    predicate: x => leaveTypeIds.Contains(x.Id),
                    cancellationToken: cancellationToken)
                : null;

            var leaveTypeDtos = leaveTypesPage != null
                ? _mapper.Map<GetListLeaveTypeListItemDto[]>(leaveTypesPage.Items)
                : Array.Empty<GetListLeaveTypeListItemDto>();

            var calculated = TimesheetCalculator.CalculateDailyTimesheet(
                employee.Id,
                day,
                ruleSetDto,
                timeEntryDtos,
                employeeLeaveDtos,
                leaveTypeDtos);

            var createCommand = new CreateTimesheetCommand
            {
                EmployeeId = calculated.EmployeeId,
                WorkDate = calculated.WorkDate,
                ShiftId = calculated.ShiftId,
                PlannedMinutes = calculated.PlannedMinutes,
                ActualMinutes = calculated.ActualMinutes,
                OvertimeMinutes = calculated.OvertimeMinutes,
                MissingMinutes = calculated.MissingMinutes,
                LateArrivalMinutes = calculated.LateArrivalMinutes,
                EarlyArrivalMinutes = calculated.EarlyArrivalMinutes,
                EarlyLeaveMinutes = calculated.EarlyLeaveMinutes,
                LateLeaveMinutes = calculated.LateLeaveMinutes,
                Status = calculated.Status,
                EmployeeLeaveId = calculated.EmployeeLeaveId
            };

            var created = await _mediator.Send(createCommand, cancellationToken);

            return created;
        }
    }
}
