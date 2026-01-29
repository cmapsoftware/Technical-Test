using Timesheet.Models.Domain;

namespace Timesheet.Repositories;

public interface ITimesheetRepository
{
    IEnumerable<TimesheetEntry> GetAll();
    TimesheetEntry? Get(int id);
    TimesheetEntry? Add(TimesheetEntryInsert timesheetEntryInsert);
    TimesheetEntry? Update(TimesheetEntry timesheetEntry);
    bool Delete(int id);
}