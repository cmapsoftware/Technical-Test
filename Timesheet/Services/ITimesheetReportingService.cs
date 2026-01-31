using Timesheet.Models.ViewModels;

namespace Timesheet.Services;

public interface ITimesheetReportingService
{
    ReportViewModel GetReportModel(int? userId, string? isoWeekIdIsoYearId);
}