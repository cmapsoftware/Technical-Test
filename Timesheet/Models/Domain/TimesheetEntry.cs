namespace Timesheet.Models.Domain;

// Represents a single row in a user's timesheet. 
// For test store UserId and ProjectId directly to keep domain simple.
// (User and Project objects not modeled in this test domain.)
public class TimesheetEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
}