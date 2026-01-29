namespace Timesheet.Models.Domain;

// I split TimesheetEntryInsert and TimesheetEntry.
// I want the clean separation and to avoid having nullable Id in the main model.
// I've not inherited TimesheetEntryInsert as I don't want TimesheetEntry, as an inherting class, to be used for inserts. (i.e. prevent Liskov Substitution Principle).
public class TimesheetEntry : TimesheetEntryBase
{
    public int Id { get; set; }
}