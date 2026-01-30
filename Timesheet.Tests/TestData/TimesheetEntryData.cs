using Timesheet.Models.Domain;

namespace Timesheet.Tests.TestData;

public static class TimesheetEntryData
{
    public const int ListPositionTimesheetEntry1 = 0;
    public const int ListPositionTimesheetEntry2 = 1;
    public const int ListPositionTimesheetEntry3 = 2;

    public static List<TimesheetEntryInsert> ListTimesheetEntryInsert =>
    [
        new TimesheetEntryInsert {
            UserId = 101,
            ProjectId = 201,
            Date = new DateTime(2024, 6, 1),
            Hours = 8.0m,
            Description = "Worked on project tasks"
        },
        new TimesheetEntryInsert {
            UserId = 102,
            ProjectId = 202,
            Date = new DateTime(2024, 6, 2),
            Hours = 6.5m,
            Description = "Client meeting and documentation"
        },
        new TimesheetEntryInsert {
            UserId = 103,
            ProjectId = 203,
            Date = new DateTime(2024, 6, 3),
            Hours = 7.0m,
            Description = null
        }
    ];

    public static List<TimesheetEntry> ListTimesheetEntry =>
    [
        new TimesheetEntry {
            Id = 0,
            UserId = 101,
            ProjectId = 201,
            Date = new DateTime(2024, 6, 1),
            Hours = 8.0m,
            Description = "Worked on project tasks"
        },
        new TimesheetEntry {
            Id = 1,
            UserId = 102,
            ProjectId = 202,
            Date = new DateTime(2024, 6, 2),
            Hours = 6.5m,
            Description = "Client meeting and documentation"
        },
        new TimesheetEntry {
            Id = 2,
            UserId = 103,
            ProjectId = 203,
            Date = new DateTime(2024, 6, 3),
            Hours = 7.0m,
            Description = null
        }
    ];
}