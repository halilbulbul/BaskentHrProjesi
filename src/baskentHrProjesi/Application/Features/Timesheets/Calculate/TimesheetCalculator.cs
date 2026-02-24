using Application.Features.EmployeeLeaves.Queries.GetList;
using Application.Features.LeaveTypes.Queries.GetList;
using Application.Features.Shifts.Queries.GetById;
using Application.Features.TimeEntries.Queries.GetList;
using Application.Features.Timesheets.Queries.GetList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.Features.Timesheets.Calculate
{
    public static class TimesheetCalculator
    {
        public static GetListTimesheetListItemDto CalculateDailyTimesheet(
            int employeeId,
            DateTime workDate,
            GetByIdShiftResponse ruleSet,
            IEnumerable<GetListTimeEntryListItemDto> timeEntries,
            IEnumerable<GetListEmployeeLeaveListItemDto> employeeLeaves,
            IEnumerable<GetListLeaveTypeListItemDto> leaveTypes)
        {
            var date = workDate.Date;

            var shiftStart = date + ruleSet.ShiftStartTime;
            var shiftEnd = date + ruleSet.ShiftEndTime;

            DateTime? breakStart = null;
            DateTime? breakEnd = null;

            if (ruleSet.BreakStartTime.HasValue && ruleSet.BreakEndTime.HasValue)
            {
                breakStart = date + ruleSet.BreakStartTime.Value;
                breakEnd = date + ruleSet.BreakEndTime.Value;
            }

            var totalShiftMinutes = (ruleSet.ShiftEndTime - ruleSet.ShiftStartTime).TotalMinutes;
            var breakMinutes = ruleSet.BreakStartTime.HasValue && ruleSet.BreakEndTime.HasValue
                ? (ruleSet.BreakEndTime.Value - ruleSet.BreakStartTime.Value).TotalMinutes
                : 0;

            var plannedMinutes = (int)(totalShiftMinutes - breakMinutes);

            var leaveForDay = employeeLeaves
                .Where(l =>
                    l.EmployeeId == employeeId &&
                    l.StartDate.Date <= date &&
                    l.EndDate.Date >= date)
                .OrderBy(l => l.StartDate)
                .FirstOrDefault();

            if (leaveForDay != null)
            {
                var leaveType = leaveTypes.FirstOrDefault(x => x.Id == leaveForDay.LeaveTypeId);
                var isPaid = leaveType?.IsPaid ?? true;

                if (isPaid)
                {
                    return new GetListTimesheetListItemDto
                    {
                        Id = 0,
                        EmployeeId = employeeId,
                        WorkDate = date,
                        ShiftId = ruleSet.Id,
                        PlannedMinutes = plannedMinutes,
                        ActualMinutes = plannedMinutes,
                        OvertimeMinutes = 0,
                        MissingMinutes = 0,
                        LateArrivalMinutes = 0,
                        EarlyArrivalMinutes = 0,
                        EarlyLeaveMinutes = 0,
                        LateLeaveMinutes = 0,
                        Status = leaveType?.Name,
                        EmployeeLeaveId = leaveForDay.Id
                    };
                }

                return new GetListTimesheetListItemDto
                {
                    Id = 0,
                    EmployeeId = employeeId,
                    WorkDate = date,
                    ShiftId = ruleSet.Id,
                    PlannedMinutes = 0,
                    ActualMinutes = 0,
                    OvertimeMinutes = 0,
                    MissingMinutes = 0,
                    LateArrivalMinutes = 0,
                    EarlyArrivalMinutes = 0,
                    EarlyLeaveMinutes = 0,
                    LateLeaveMinutes = 0,
                    Status = leaveType?.Name,
                    EmployeeLeaveId = leaveForDay.Id
                };
            }

            var orderedEntries = timeEntries
                .Where(x => x.EmployeeId == employeeId && x.EventTime.Date == date)
                .OrderBy(x => x.EventTime)
                .ToList();

            DateTime? currentIn = null;
            var intervals = new List<(DateTime In, DateTime Out)>();

            foreach (var entry in orderedEntries)
            {
                if (!string.IsNullOrWhiteSpace(entry.Direction) &&
                    entry.Direction.Equals("IN", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentIn == null)
                        currentIn = entry.EventTime;
                }
                else if (!string.IsNullOrWhiteSpace(entry.Direction) &&
                         entry.Direction.Equals("OUT", StringComparison.OrdinalIgnoreCase))
                {
                    if (currentIn != null && entry.EventTime > currentIn.Value)
                    {
                        intervals.Add((currentIn.Value, entry.EventTime));
                        currentIn = null;
                    }
                }
            }

            var rawMinutes = intervals.Sum(i => (i.Out - i.In).TotalMinutes);

            double breakOverlapMinutes = 0;
            if (breakStart.HasValue && breakEnd.HasValue)
            {
                foreach (var interval in intervals)
                {
                    var overlapStart = interval.In > breakStart.Value ? interval.In : breakStart.Value;
                    var overlapEnd = interval.Out < breakEnd.Value ? interval.Out : breakEnd.Value;
                    if (overlapEnd > overlapStart)
                        breakOverlapMinutes += (overlapEnd - overlapStart).TotalMinutes;
                }
            }

            var actualMinutes = (int)Math.Max(0, rawMinutes - breakOverlapMinutes);

            var firstIn = intervals.Any() ? intervals.Min(i => i.In) : (DateTime?)null;
            var lastOut = intervals.Any() ? intervals.Max(i => i.Out) : (DateTime?)null;

            int lateArrivalMinutes = 0;
            int earlyArrivalMinutes = 0;
            int earlyLeaveMinutes = 0;
            int lateLeaveMinutes = 0;

            if (firstIn.HasValue)
            {
                var lateBorder = shiftStart.AddMinutes(ruleSet.LateArrivalToleranceMinutes);
                if (firstIn.Value > lateBorder)
                    lateArrivalMinutes = (int)Math.Max(0, (firstIn.Value - lateBorder).TotalMinutes);

                var earlyBorder = shiftStart.AddMinutes(-ruleSet.EarlyArrivalToleranceMinutes);
                if (firstIn.Value < earlyBorder)
                    earlyArrivalMinutes = (int)Math.Max(0, (earlyBorder - firstIn.Value).TotalMinutes);
            }

            if (lastOut.HasValue)
            {
                var earlyLeaveBorder = shiftEnd.AddMinutes(-ruleSet.EarlyLeaveToleranceMinutes);
                if (lastOut.Value < earlyLeaveBorder)
                    earlyLeaveMinutes = (int)Math.Max(0, (earlyLeaveBorder - lastOut.Value).TotalMinutes);

                var lateLeaveBorder = shiftEnd.AddMinutes(ruleSet.LateLeaveToleranceMinutes);
                if (lastOut.Value > lateLeaveBorder)
                    lateLeaveMinutes = (int)Math.Max(0, (lastOut.Value - lateLeaveBorder).TotalMinutes);
            }

            int overtimeMinutes = 0;
            int missingMinutes = 0;

            double toleratedMissing = 0;
            double toleratedOvertime = 0;

            if (firstIn.HasValue)
            {
                if (firstIn.Value > shiftStart)
                {
                    var deltaStart = (firstIn.Value - shiftStart).TotalMinutes;
                    var toleratedStart = Math.Min(deltaStart, ruleSet.LateArrivalToleranceMinutes);
                    toleratedMissing += toleratedStart;
                }
                else if (firstIn.Value < shiftStart)
                {
                    var deltaEarly = (shiftStart - firstIn.Value).TotalMinutes;
                    var toleratedEarly = Math.Min(deltaEarly, ruleSet.EarlyArrivalToleranceMinutes);
                    toleratedOvertime += toleratedEarly;
                }
            }

            if (lastOut.HasValue)
            {
                if (lastOut.Value < shiftEnd)
                {
                    var deltaEnd = (shiftEnd - lastOut.Value).TotalMinutes;
                    var toleratedEnd = Math.Min(deltaEnd, ruleSet.EarlyLeaveToleranceMinutes);
                    toleratedMissing += toleratedEnd;
                }
                else if (lastOut.Value > shiftEnd)
                {
                    var deltaLate = (lastOut.Value - shiftEnd).TotalMinutes;
                    var toleratedLate = Math.Min(deltaLate, ruleSet.LateLeaveToleranceMinutes);
                    toleratedOvertime += toleratedLate;
                }
            }

            if (actualMinutes > plannedMinutes)
            {
                overtimeMinutes = (int)Math.Max(0, actualMinutes - plannedMinutes - toleratedOvertime);
            }
            else
            {
                missingMinutes = (int)Math.Max(0, plannedMinutes - actualMinutes - toleratedMissing);
            }

            var status = "Normal";

            if (!intervals.Any())
            {
                var hasIn = orderedEntries.Any(e =>
                    !string.IsNullOrWhiteSpace(e.Direction) &&
                    e.Direction.Equals("IN", StringComparison.OrdinalIgnoreCase));

                var hasOut = orderedEntries.Any(e =>
                    !string.IsNullOrWhiteSpace(e.Direction) &&
                    e.Direction.Equals("OUT", StringComparison.OrdinalIgnoreCase));

                if (hasIn && !hasOut)
                    status = "Çıkış Yok";
                else if (!hasIn && hasOut)
                    status = "Giriş Yok";
                else
                    status = "Giriş Yok";

                actualMinutes = 0;
                overtimeMinutes = 0;
                missingMinutes = plannedMinutes;
                lateArrivalMinutes = 0;
                earlyLeaveMinutes = 0;
                earlyArrivalMinutes = 0;
                lateLeaveMinutes = 0;
            }
            else if (missingMinutes > 0)
            {
                status = "Eksik Mesai";
            }
            else if (overtimeMinutes > 0)
            {
                status = "Fazla Mesai";
            }

            return new GetListTimesheetListItemDto
            {
                Id = 0,
                EmployeeId = employeeId,
                WorkDate = date,
                ShiftId = ruleSet.Id,
                PlannedMinutes = plannedMinutes,
                ActualMinutes = actualMinutes,
                OvertimeMinutes = overtimeMinutes,
                MissingMinutes = missingMinutes,
                LateArrivalMinutes = lateArrivalMinutes,
                EarlyArrivalMinutes = earlyArrivalMinutes,
                EarlyLeaveMinutes = earlyLeaveMinutes,
                LateLeaveMinutes = lateLeaveMinutes,
                Status = status,
                EmployeeLeaveId = null
            };
        }
    }
}
