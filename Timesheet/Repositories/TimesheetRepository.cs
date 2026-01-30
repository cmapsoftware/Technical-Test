using Timesheet.Models.Domain;

namespace Timesheet.Repositories;

// This repository stores timesheet entries in memory for the duration of the application's lifetime, simulating basic CRUD operations without a database.
// I've decided I do not want any methods to mutate the input parameters directly, and for each to pass back a new or updated instance or a success flag.
// (I may not have a use for these returned values currently, but for testing alone they are useful)
public class TimesheetRepository : ITimesheetRepository
{
    private readonly List<TimesheetEntry> _listTimesheetEntries = [];

    // Private counter used to generate unique Ids for new entries mirroring DB auto-increment.
    // Avoid scanning list for highest Id. Also if highest row was deleted we do not want to reuse that ID.
    private int _nextId = 1;

    public IEnumerable<TimesheetEntry> GetAll()
    {
        return _listTimesheetEntries;
    }

    public TimesheetEntry? GetById(int timesheetId)
    {
        return _listTimesheetEntries.FirstOrDefault(timesheetEntry => timesheetEntry.Id == timesheetId);
    }

    public TimesheetEntry? GetDuplicateTimesheetRow(int userId, int projectId, DateTime date, int? currentRowId)
    {
        return _listTimesheetEntries.FirstOrDefault(timesheetEntry =>
                timesheetEntry.UserId == userId &&
                timesheetEntry.ProjectId == projectId &&
                timesheetEntry.Date == date &&
                (currentRowId == null || timesheetEntry.Id != currentRowId) // Exclude current row as duplicate if updating
            );
    }

    public TimesheetEntry Add(TimesheetEntryInsert timesheetEntryInsert)
    {
        var newTimesheetEntry = new TimesheetEntry
        {
            Id = _nextId++,
            UserId = timesheetEntryInsert.UserId,
            ProjectId = timesheetEntryInsert.ProjectId,
            Date = timesheetEntryInsert.Date,
            Hours = timesheetEntryInsert.Hours,
            Description = timesheetEntryInsert.Description
        };

        _listTimesheetEntries.Add(newTimesheetEntry);
        return newTimesheetEntry;
    }

    public TimesheetEntry? Update(TimesheetEntry timesheetEntry)
    {
        var existingTimesheetEntry = GetById(timesheetEntry.Id);
        if (existingTimesheetEntry == null)
        {
            return null;
        }

        existingTimesheetEntry.UserId = timesheetEntry.UserId;
        existingTimesheetEntry.ProjectId = timesheetEntry.ProjectId;
        existingTimesheetEntry.Date = timesheetEntry.Date;
        existingTimesheetEntry.Hours = timesheetEntry.Hours;
        existingTimesheetEntry.Description = timesheetEntry.Description;
        return existingTimesheetEntry;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing == null)
        {
            return false;
        }
        return _listTimesheetEntries.Remove(existing);
    }
}