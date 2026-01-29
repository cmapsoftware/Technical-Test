namespace Timesheet.Models.Domain;

// Represents a single timesheet row to be inserted (i.e. Without an Id, it does not have one before insertion into repository)
// For this test code I've stored UserId and ProjectId directly as ints to keep domain simple, both would normally be modelled.
public class TimesheetEntryBase
{
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public DateTime Date { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
}