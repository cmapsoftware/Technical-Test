using Timesheet.Models.Domain;
using Timesheet.Models.Dto;

namespace Timesheet.Helpers
{
    // TODO: Use automapping library instead
    public static class MappingHelper
    {
        public static IEnumerable<TimesheetEntryDto> MapTimesheetEntriesToTimesheetEntryDtos(IEnumerable<TimesheetEntry> timesheetEntries) 
        {
            return timesheetEntries.Select(e => new TimesheetEntryDto
            {
                Id = e.Id,
                UserId = e.UserId,
                ProjectId = e.ProjectId,
                Date = e.Date,
                Hours = e.Hours,
                Description = e.Description
            });
        }
    }
}
