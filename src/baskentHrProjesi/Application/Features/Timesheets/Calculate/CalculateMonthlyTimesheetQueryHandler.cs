using Application.Features.EmployeeLeaves.Queries.GetList;
using Application.Features.LeaveTypes.Queries.GetList;
using Application.Features.Shifts.Queries.GetById;
using Application.Features.TimeEntries.Queries.GetList;
using Application.Features.Timesheets.Commands.Create;
using Application.Features.Timesheets.Commands.Update;
using Application.Features.Timesheets.Queries.GetList;
using Application.Services.Repositories;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Timesheets.Calculate
{
    public class CalculateMonthlyTimesheetQueryHandler
        : IRequestHandler<CalculateMonthlyTimesheetQuery, IList<GetListTimesheetListItemDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IShiftRepository _ShiftRepository;
        private readonly ITimeEntryRepository _timeEntryRepository;
        private readonly IEmployeeLeaveRepository _employeeLeaveRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public CalculateMonthlyTimesheetQueryHandler(
            IEmployeeRepository employeeRepository,
            IShiftRepository ShiftRepository,
            ITimeEntryRepository timeEntryRepository,
            IEmployeeLeaveRepository employeeLeaveRepository,
            ILeaveTypeRepository leaveTypeRepository,
            ITimesheetRepository timesheetRepository,
            IMediator mediator,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _ShiftRepository = ShiftRepository;
            _timeEntryRepository = timeEntryRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _timesheetRepository = timesheetRepository;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<IList<GetListTimesheetListItemDto>> Handle(
            CalculateMonthlyTimesheetQuery request,
            CancellationToken cancellationToken)
        {
            if (request == null ||
                request.EmployeeIds == null ||
                request.EmployeeIds.Count == 0 ||
                request.Year <= 0 ||
                request.Month <= 0 || request.Month > 12)
            {
                throw new Exception("Geçersiz parametreler.");
            }

            var monthStart = new DateTime(request.Year, request.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var result = new List<GetListTimesheetListItemDto>();
            var employeeIds = request.EmployeeIds.Distinct().ToList();

            foreach (var employeeId in employeeIds)
            {
                var employee = await _employeeRepository.GetAsync(
                    e => e.Id == employeeId,
                    cancellationToken: cancellationToken);

                if (employee == null)
                    continue;

                if (employee.ShiftId == null)
                    continue;

                var ruleSetEntity = await _ShiftRepository.GetAsync(
                    r => r.Id == employee.ShiftId,
                    cancellationToken: cancellationToken);

                if (ruleSetEntity == null)
                    continue;

                var ruleSetDto = _mapper.Map<GetByIdShiftResponse>(ruleSetEntity);

                for (var workDate = monthStart; workDate < monthEnd; workDate = workDate.AddDays(1))
                {
                    var dayStart = workDate.Date;
                    var nextDay = dayStart.AddDays(1);

                    var timeEntriesPage = await _timeEntryRepository.GetListAsync(
                        predicate: t => t.EmployeeId == employeeId &&
                                        t.EventTime >= dayStart &&
                                        t.EventTime < nextDay,
                        cancellationToken: cancellationToken);

                    var timeEntryDtos = _mapper.Map<GetListTimeEntryListItemDto[]>(timeEntriesPage.Items);

                    if ((workDate.DayOfWeek == DayOfWeek.Saturday || workDate.DayOfWeek == DayOfWeek.Sunday) &&
                        !timeEntryDtos.Any())
                    {
                        continue;
                    }

                    var employeeLeavesPage = await _employeeLeaveRepository.GetListAsync(
                        predicate: l => l.EmployeeId == employeeId &&
                                        l.EndDate >= dayStart &&
                                        l.StartDate <= nextDay,
                        cancellationToken: cancellationToken);

                    var employeeLeaveDtos = _mapper.Map<GetListEmployeeLeaveListItemDto[]>(employeeLeavesPage.Items);

                    var leaveTypeIds = employeeLeaveDtos
                        .Select(l => l.LeaveTypeId)
                        .Distinct()
                        .ToArray();

                    GetListLeaveTypeListItemDto[] leaveTypeDtos;

                    if (leaveTypeIds.Any())
                    {
                        var leaveTypesPage = await _leaveTypeRepository.GetListAsync(
                            predicate: x => leaveTypeIds.Contains(x.Id),
                            cancellationToken: cancellationToken);

                        leaveTypeDtos = _mapper.Map<GetListLeaveTypeListItemDto[]>(leaveTypesPage.Items);
                    }
                    else
                    {
                        leaveTypeDtos = Array.Empty<GetListLeaveTypeListItemDto>();
                    }

                    var calculated = TimesheetCalculator.CalculateDailyTimesheet(
                        employeeId,
                        dayStart,
                        ruleSetDto,
                        timeEntryDtos,
                        employeeLeaveDtos,
                        leaveTypeDtos);

                    if ((workDate.DayOfWeek == DayOfWeek.Saturday || workDate.DayOfWeek == DayOfWeek.Sunday) &&
                        timeEntryDtos.Any())
                    {
                        calculated.Status = "Hafta Sonu Mesaisi";
                    }

                    var existing = await _timesheetRepository.GetAsync(
                        t => t.EmployeeId == employeeId &&
                             t.WorkDate >= dayStart &&
                             t.WorkDate < nextDay,
                        cancellationToken: cancellationToken);

                    if (existing != null)
                    {
                        var updateCommand = new UpdateTimesheetCommand
                        {
                            Id = existing.Id,
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

                        var updated = await _mediator.Send(updateCommand, cancellationToken);

                        var dto = new GetListTimesheetListItemDto
                        {
                            Id = updated.Id,
                            EmployeeId = updated.EmployeeId,
                            WorkDate = updated.WorkDate,
                            ShiftId = updated.ShiftId,
                            PlannedMinutes = updated.PlannedMinutes,
                            ActualMinutes = updated.ActualMinutes,
                            OvertimeMinutes = updated.OvertimeMinutes,
                            MissingMinutes = updated.MissingMinutes,
                            LateArrivalMinutes = updated.LateArrivalMinutes,
                            EarlyArrivalMinutes = updated.EarlyArrivalMinutes,
                            EarlyLeaveMinutes = updated.EarlyLeaveMinutes,
                            LateLeaveMinutes = updated.LateLeaveMinutes,
                            Status = updated.Status,
                            EmployeeLeaveId = updated.EmployeeLeaveId
                        };

                        result.Add(dto);
                    }
                    else
                    {
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

                        var dto = new GetListTimesheetListItemDto
                        {
                            Id = created.Id,
                            EmployeeId = created.EmployeeId,
                            WorkDate = created.WorkDate,
                            ShiftId = created.ShiftId,
                            PlannedMinutes = created.PlannedMinutes,
                            ActualMinutes = created.ActualMinutes,
                            OvertimeMinutes = created.OvertimeMinutes,
                            MissingMinutes = created.MissingMinutes,
                            LateArrivalMinutes = created.LateArrivalMinutes,
                            EarlyArrivalMinutes = created.EarlyArrivalMinutes,
                            EarlyLeaveMinutes = created.EarlyLeaveMinutes,
                            LateLeaveMinutes = created.LateLeaveMinutes,
                            Status = created.Status,
                            EmployeeLeaveId = created.EmployeeLeaveId
                        };

                        result.Add(dto);
                    }
                }
            }

            return result;
        }
    }
}
