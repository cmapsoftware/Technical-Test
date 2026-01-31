using System.Globalization;
using Timesheet.Models.Domain;
using Timesheet.Models.Dto;

namespace Timesheet.Repositories;

// This repository stores timesheet entries in memory for the duration of the application's lifetime, simulating basic CRUD operations without a database.
// I've decided I do not want any methods to mutate the input parameters directly, and for each to pass back a new or updated instance or a success flag.
// (I may not have a use for these returned values currently, but for testing alone they are useful)
public class TimesheetRepository : ITimesheetRepository
{
    private readonly List<TimesheetEntry> _listTimesheetEntries = [];
    private int _nextId = 1;

    #region Public methods

    public IEnumerable<TimesheetEntry> GetAllTimesheetEntries()
    {
        return _listTimesheetEntries;
    }

    public IEnumerable<TimesheetEntry> GetReportTimesheetEntries(int userId, int isoWeek, int isoYear)
    {
        return _listTimesheetEntries.Where(timesheetEntry =>
            timesheetEntry.UserId == userId &&
            ISOWeek.GetWeekOfYear(timesheetEntry.Date) == isoWeek &&
            ISOWeek.GetYear(timesheetEntry.Date) == isoYear
        )
            .OrderBy(timesheetEntry => timesheetEntry.ProjectId)
            .ThenBy(timesheetEntry => timesheetEntry.Date);
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

    public IEnumerable<int> GetAllUserIds()
    {
        return _listTimesheetEntries
            .Select(entry => entry.UserId)
            .Distinct()
            .Order();
    }

    public IEnumerable<IsoWeekDto> GetAllIsoWeeks()
    {
        var dates = _listTimesheetEntries
            .Select(entry => entry.Date)
            .Distinct();

        var results =
            dates
                .Select(d => new
                {
                    Date = d,
                    Year = ISOWeek.GetYear(d),
                    Week = ISOWeek.GetWeekOfYear(d)
                })
                .GroupBy(x => new { x.Year, x.Week })
                .Select(g =>
                {
                    return new IsoWeekDto
                    {
                        IsoYearId = g.Key.Year,
                        IsoWeekId = g.Key.Week,
                        WeekRange = $"{DateOnly.FromDateTime(ISOWeek.ToDateTime(g.Key.Year, g.Key.Week, DayOfWeek.Monday))}" +
                                    $" - {DateOnly.FromDateTime(ISOWeek.ToDateTime(g.Key.Year, g.Key.Week, DayOfWeek.Sunday))}"
                    };
                })
                .OrderBy(x => x.IsoYearId)
                .ThenBy(x => x.IsoWeekId)
                .ToList();

        return results;
    }

    #endregion Public methods

    #region Private methods

    private TimesheetEntry? GetById(int timesheetId)
    {
        return _listTimesheetEntries.FirstOrDefault(timesheetEntry => timesheetEntry.Id == timesheetId);
    }

    #endregion Private methods
}