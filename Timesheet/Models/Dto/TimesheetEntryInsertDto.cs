using System.ComponentModel.DataAnnotations;
using Timesheet.Validation;

namespace Timesheet.Models.Dto;

public class TimesheetEntryInsertDto
{
    [Range(1, int.MaxValue, ErrorMessage = "UserId must be a positive integer.")]
    public int UserId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ProjectId must be a positive integer.")]
    public int ProjectId { get; set; }

    [DataType(DataType.Date)]
    [DateNotInFuture(ErrorMessage = "Date cannot be in the future.")]
    public DateTime Date { get; set; }

    [Range(0.25, 12, ErrorMessage = "Hours must be between 0.25 and 12.")] // assumed reasonable working time bounds per day
    public decimal Hours { get; set; }

    [MaxLength(100, ErrorMessage = "Description cannot exceed 100 characters.")] // assumed a reasonable size limit
    public string? Description { get; set; }
}