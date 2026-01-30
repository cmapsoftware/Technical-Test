using Timesheet.Models.Dto;

namespace Timesheet.Services;

public interface ITimesheetService
{
    IEnumerable<TimesheetEntryDto> GetAll();
    TimesheetEntryDto? Add(TimesheetEntryInsertDto timesheetEntryInsertDto);
    TimesheetEntryDto? Update(TimesheetEntryDto timesheetEntryDto);
    bool Delete(int id);
    void Validate(object dto);
}