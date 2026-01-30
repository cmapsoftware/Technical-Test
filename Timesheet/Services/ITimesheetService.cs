using Timesheet.Models.Dto;

namespace Timesheet.Services;

public interface ITimesheetService
{
    IEnumerable<TimesheetEntryDto> GetAll();
    TimesheetEntryDto? GetDuplicateTimesheetRow(int userId, int projectId, DateTime Date, int? currentRowId);
    TimesheetEntryDto? Add(TimesheetEntryInsertDto timesheetEntryInsertDto);
    TimesheetEntryDto? Update(TimesheetEntryDto timesheetEntryDto);
    bool Delete(int id);
    void Validate(object dto);
}