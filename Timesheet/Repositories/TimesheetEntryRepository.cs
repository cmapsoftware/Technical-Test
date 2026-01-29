using Timesheet.Models.Domain;

namespace Timesheet.Repositories;

// This repository stores timesheet entries in memory for the duration of the application's lifetime.
// simulating basic CRUD operations without a database.
public class TimesheetEntryRepository : ITimesheetEntryRepository
{
    private readonly List<TimesheetEntry> _listTimesheetEntries = new();

    // Private counter used to generate unique Ids for new entries mirroring DB auto-increment.
    // Avoid scanning list for highest Id. Also if highest row was deleted we do not want to reuse that ID.
    private int _nextId = 1;

    public IEnumerable<TimesheetEntry> GetAll() => _listTimesheetEntries;

    public TimesheetEntry? Get(int timesheetId) =>
        _listTimesheetEntries.FirstOrDefault(timesheetEntry => timesheetEntry.Id == timesheetId);

    public void Add(TimesheetEntry timesheetEntry)
    {
        timesheetEntry.Id = _nextId++;
        _listTimesheetEntries.Add(timesheetEntry);
    }

    public void Update(TimesheetEntry timesheetEntry)
    {
        var existing = Get(timesheetEntry.Id);
        if (existing == null)
        {
            return;
        }

        existing.UserId = timesheetEntry.UserId;
        existing.ProjectId = timesheetEntry.ProjectId;
        existing.Date = timesheetEntry.Date;
        existing.Hours = timesheetEntry.Hours;
        existing.Description = timesheetEntry.Description;
    }

    public void Delete(int id)
    {
        var existing = Get(id);
        if (existing != null)
        {
            _listTimesheetEntries.Remove(existing);
        }
    }
}