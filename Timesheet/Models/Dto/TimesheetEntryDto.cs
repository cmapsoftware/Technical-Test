using System.ComponentModel.DataAnnotations;

namespace Timesheet.Models.Dto;

public class TimesheetEntryDto : TimesheetEntryInsertDto
{
    [Required]
    public int Id { get; set; }
}