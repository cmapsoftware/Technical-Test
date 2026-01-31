namespace Timesheet.Models.Dto
{
    public class IsoWeekDto
    {
        public int IsoYearId { get; set; }
        public int IsoWeekId { get; set; }
        public string? WeekRange { get; set; }
    }
}