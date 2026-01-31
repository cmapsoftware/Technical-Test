using Timesheet.Models.Domain;
using Timesheet.Models.Dto;

namespace Timesheet.Repositories;

// Note: typically repository methods would be async when interacting with a database, but since this is an in-memory implementation, they are synchronous.
public interface ITimesheetRepository
{
    IEnumerable<TimesheetEntry> GetAllTimesheetEntries();
    IEnumerable<TimesheetEntry> GetReportTimesheetEntries(int userId, int isoWeek, int isoYear);
    TimesheetEntry? GetDuplicateTimesheetRow(int userId, int projectId, DateTime date, int? currentRowId);
    TimesheetEntry? Add(TimesheetEntryInsert timesheetEntryInsert);
    TimesheetEntry? Update(TimesheetEntry timesheetEntry);
    bool Delete(int id);
    IEnumerable<int> GetAllUserIds();
    IEnumerable<IsoWeekDto> GetAllIsoWeeks();
}