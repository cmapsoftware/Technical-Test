using System.ComponentModel.DataAnnotations;
using Timesheet.Models.Dto;
using Timesheet.Repositories;

namespace Timesheet.Validation;

public class TimesheetEntryValidator(ITimesheetRepository timesheetRepository) : ITimesheetEntryValidator
{
    private readonly ITimesheetRepository _timesheetRepository = timesheetRepository;

    public ValidationResult? ValidateUnique(TimesheetEntryInsertDto dto, int? rowId = null)
    {
        if (_timesheetRepository.GetDuplicateTimesheetRow(dto.UserId, dto.ProjectId, dto.Date, rowId) != null)
        {
            var message = rowId.HasValue
                ? $"A timesheet entry already exists with UserId={dto.UserId}, ProjectId={dto.ProjectId} and Date={DateOnly.FromDateTime(dto.Date)}. Update failed for Row ID #{rowId}."
                : $"A timesheet entry already exists with UserId={dto.UserId}, ProjectId={dto.ProjectId} and Date={DateOnly.FromDateTime(dto.Date)}. Insert failed";
            return new ValidationResult(message);
        }
        return ValidationResult.Success;
    }
}