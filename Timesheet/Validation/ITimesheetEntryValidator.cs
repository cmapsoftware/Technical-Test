using System.ComponentModel.DataAnnotations;
using Timesheet.Models.Dto;

namespace Timesheet.Validation;

public interface ITimesheetEntryValidator
{
    ValidationResult? ValidateUnique(TimesheetEntryInsertDto dto, int? rowId = null);
}