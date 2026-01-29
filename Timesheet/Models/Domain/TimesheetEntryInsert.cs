namespace Timesheet.Models.Domain;

// TimesheetEntryInsert (with no Id yet) inherits TimesheetEntryBase with no changes.
// If TimesheetEntry inherited from TimesheetEntryInsert it would allow passing full TimesheetEntry to Add() method which is not desired.
public class TimesheetEntryInsert : TimesheetEntryBase
{
}
