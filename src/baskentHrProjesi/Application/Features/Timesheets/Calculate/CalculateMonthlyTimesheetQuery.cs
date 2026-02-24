using System.Collections.Generic;
using Application.Features.Timesheets.Queries.GetList;
using MediatR;

namespace Application.Features.Timesheets.Calculate
{
    public class CalculateMonthlyTimesheetQuery : IRequest<IList<GetListTimesheetListItemDto>>
    {
        public IList<int> EmployeeIds { get; set; } = new List<int>();
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
