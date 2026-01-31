using Timesheet.Models.Dto;

namespace Timesheet.Models.ViewModels;

public class TimesheetRowViewModel
{
    public TimesheetEntryDto? TimesheetEntryDto { get; set; }
    public bool IsEditable { get; set; }
}
