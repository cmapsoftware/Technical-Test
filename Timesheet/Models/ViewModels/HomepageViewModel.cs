using Timesheet.Models.Dto;

namespace Timesheet.Models.ViewModels;

public class HomepageViewModel
{
    public List<TimesheetEntryDto>? TimesheetEntries { get; set; }
    public TimesheetEntryInsertDto? NewTimesheetEntry { get; set; }
}