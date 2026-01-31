using System.Globalization;
using Timesheet.Helpers;
using Timesheet.Models.ViewModels;
using Timesheet.Repositories;

namespace Timesheet.Services;

public class TimesheetReportingService(ITimesheetRepository timesheetRepository) : ITimesheetReportingService
{
    private readonly ITimesheetRepository _timesheetRepository = timesheetRepository;

    public ReportViewModel GetReportModel(int? userId, string? isoWeekIdIsoYearId)
    {
        if (userId == null && isoWeekIdIsoYearId == null)
        {
            return new ReportViewModel
            {
                UserIds = [.. _timesheetRepository.GetAllUserIds()],
                IsoWeeks = [.. _timesheetRepository.GetAllIsoWeeks()]
            };
        }

        if (!string.IsNullOrEmpty(isoWeekIdIsoYearId)
            && userId != null
            && TryGetIsoWeekFromIsoWeekIsoYearString(isoWeekIdIsoYearId, out var isoWeek)
            && TryGetIsoYearFromIsoWeekIsoYearString(isoWeekIdIsoYearId, out var isoYear))
        {
            var timesheetEntries = _timesheetRepository.GetReportTimesheetEntries(userId.Value, isoWeek, isoYear);
            var timesheetEntryDtos = MappingHelper.MapTimesheetEntriesToTimesheetEntryDtos(timesheetEntries);

            var projectHours = timesheetEntries
                    .GroupBy(timesheetEntry => timesheetEntry.ProjectId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(te => te.Hours)).OrderBy(kvp => kvp.Key);

            return new ReportViewModel
            {
                SelectedIsoWeekIdIsoYearId = isoWeekIdIsoYearId,
                SelectedUserId = userId,
                UserIds = [.. _timesheetRepository.GetAllUserIds()],
                IsoWeeks = [.. _timesheetRepository.GetAllIsoWeeks()],
                SelectedIsoWeekRange = GetIsoWeekRangeString(isoWeekIdIsoYearId),
                TimesheetEntries = [.. timesheetEntryDtos],
                ProjectHours = projectHours
            };
        }
        else
        {
            throw new ArgumentException("Invalid ISO Week and Year format.");
        }
    }

    #region Private Helpers

    // TOODO: all methods throughout the Solution should have C# XML documentation comments like this one
    /// <summary>
    /// Takes a string representing an IsoWeek and IsoYear (e.g. "2-2026") and returns a date range string for that week (e.g. "01/01/2026 - 07/01/2026")
    /// </summary>
    /// <param name="selectedIsoWeekIdIsoYearId">is of the form "isoWeek-isoYear" (e.g. "2-2026")</param>
    /// <returns>A date range string representing an IsoWeek (e.g. "01/01/2026 - "07/01/2026")</returns>
    /// <exception cref="ArgumentException"></exception>
    private static string GetIsoWeekRangeString(string selectedIsoWeekIdIsoYearId)
    {
        if (TryGetIsoWeekFromIsoWeekIsoYearString(selectedIsoWeekIdIsoYearId, out var isoWeek)
            && TryGetIsoYearFromIsoWeekIsoYearString(selectedIsoWeekIdIsoYearId, out var isoYear))
        {
            return $"{DateOnly.FromDateTime(ISOWeek.ToDateTime(isoYear, isoWeek, DayOfWeek.Monday))}"
                + $" - {DateOnly.FromDateTime(ISOWeek.ToDateTime(isoYear, isoWeek, DayOfWeek.Sunday))}";
        }
        else
        {
            throw new ArgumentException("Invalid ISO Week and Year format.");
        }
    }

    private static bool TryGetIsoWeekFromIsoWeekIsoYearString(string value, out int isoWeek)
    {
        isoWeek = 0;
        var parts = value.Split('-');
        return parts.Length == 2 && int.TryParse(parts[0], out isoWeek);
    }

    private static bool TryGetIsoYearFromIsoWeekIsoYearString(string value, out int isoYear)
    {
        isoYear = 0;
        var parts = value.Split('-');
        return parts.Length == 2 && int.TryParse(parts[1], out isoYear);
    }

    #endregion Private Helpers
}