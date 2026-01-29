using TimesheetTest.Domain;

namespace TimesheetTest.Application.Services;

public interface ITimesheetEntryRepository
{
    IEnumerable<TimesheetEntry> GetAll();
    TimesheetEntry? Get(int id);
    void Add(TimesheetEntry entry);
    void Update(TimesheetEntry entry);
    void Delete(int id);
}