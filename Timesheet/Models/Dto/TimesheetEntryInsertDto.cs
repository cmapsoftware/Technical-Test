using System.ComponentModel.DataAnnotations;
using Timesheet.Validation;

namespace Timesheet.Models.Dto;

public class TimesheetEntryInsertDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [DateNotInFuture(ErrorMessage = "Date cannot be in the future.")]
    public DateTime Date { get; set; }

    [Required]
    [Range(0.25, 12, ErrorMessage = "Hours must be between 0.25 and 12.")] // assumed reasonable working time bounds per day
    public decimal Hours { get; set; }

    public string? Description { get; set; }
}