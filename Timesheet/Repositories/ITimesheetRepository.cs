using Timesheet.Models.Domain;

namespace Timesheet.Repositories;

// Note: typically repository methods would be async when interacting with a database, but since this is an in-memory implementation, they are synchronous.
public interface ITimesheetRepository
{
    IEnumerable<TimesheetEntry> GetAll();
    TimesheetEntry? Get(int id);
    TimesheetEntry? Add(TimesheetEntryInsert timesheetEntryInsert);
    TimesheetEntry? Update(TimesheetEntry timesheetEntry);
    bool Delete(int id);
}