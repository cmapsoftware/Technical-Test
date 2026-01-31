using Moq;
using Timesheet.Models.Domain;
using Timesheet.Models.Dto;
using Timesheet.Repositories;
using Timesheet.Services;

namespace Timesheet.Tests.Services
{
    public class TimesheetReportingServiceTests
    {
        // TODO: Copilot generated tests - review for accuracy
        [Fact]
        public void GetReportModel_NoParams_ReturnsUserIdsAndIsoWeeks()
        {
            // Arrange
            var mockRepo = new Mock<ITimesheetRepository>();
            var expectedUserIds = new List<int> { 1, 2, 3 };
            var expectedIsoWeeks = new List<IsoWeekDto>
            {
                new() { IsoWeekId = 1, IsoYearId = 2026, WeekRange = "..." }
            };

            mockRepo.Setup(r => r.GetAllUserIds()).Returns(expectedUserIds);
            mockRepo.Setup(r => r.GetAllIsoWeeks()).Returns(expectedIsoWeeks);

            var service = new TimesheetReportingService(mockRepo.Object);

            // Act
            var result = service.GetReportModel(null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUserIds, result.UserIds);
            Assert.Equal(expectedIsoWeeks, result.IsoWeeks);
            Assert.Null(result.SelectedUserId);
            Assert.Null(result.SelectedIsoWeekIdIsoYearId);
            Assert.Null(result.TimesheetEntries);
            Assert.Null(result.ProjectHours);
        }

        [Fact]
        public void GetReportModel_WithValidWeekAndUser_ReturnsPopulatedModelAndSortedProjectHours()
        {
            // Arrange
            var mockRepo = new Mock<ITimesheetRepository>();

            // Setup lookups
            var allUserIds = new List<int> { 1, 2 };
            var allIsoWeeks = new List<IsoWeekDto> { new() { IsoWeekId = 1, IsoYearId = 2026, WeekRange = "..." } };
            mockRepo.Setup(r => r.GetAllUserIds()).Returns(allUserIds);
            mockRepo.Setup(r => r.GetAllIsoWeeks()).Returns(allIsoWeeks);

            // Create timesheet entries returned for the report (intentionally unordered by project id)
            var timesheetEntries = new List<TimesheetEntry>
            {
                new() { Id = 10, UserId = 1, ProjectId = 2, Date = new DateTime(2026,1,2), Hours = 3.5m, Description = "A" },
                new() { Id = 11, UserId = 1, ProjectId = 1, Date = new DateTime(2026,1,3), Hours = 4.0m, Description = "B" },
                // another row for project 1 to validate summation
                new() { Id = 12, UserId = 1, ProjectId = 1, Date = new DateTime(2026,1,4), Hours = 2.5m, Description = "C" }
            };

            mockRepo.Setup(r => r.GetReportTimesheetEntries(1, 1, 2026)).Returns(timesheetEntries);

            var service = new TimesheetReportingService(mockRepo.Object);
            var isoWeekIdIsoYearId = "1-2026";

            // Act
            var result = service.GetReportModel(1, isoWeekIdIsoYearId);

            // Build expected SelectedIsoWeekRange using same logic as production code to avoid culture issues
            var isoWeek = 1;
            var isoYear = 2026;
            var expectedRange = $"{DateOnly.FromDateTime(System.Globalization.ISOWeek.ToDateTime(isoYear, isoWeek, DayOfWeek.Monday))}"
                                + $" - {DateOnly.FromDateTime(System.Globalization.ISOWeek.ToDateTime(isoYear, isoWeek, DayOfWeek.Sunday))}";

            // Assert
            Assert.Equal(isoWeekIdIsoYearId, result.SelectedIsoWeekIdIsoYearId);
            Assert.Equal(1, result.SelectedUserId);
            Assert.Equal(allUserIds, result.UserIds);
            Assert.Equal(allIsoWeeks, result.IsoWeeks);
            Assert.Equal(expectedRange, result.SelectedIsoWeekRange);

            // Timesheet entries are mapped to DTOs
            Assert.NotNull(result.TimesheetEntries);
            Assert.Equal(3, result.TimesheetEntries.Count);

            // Project hours should be summed per project and ordered by project id ascending
            Assert.NotNull(result.ProjectHours);
            var projectHoursList = result.ProjectHours.ToList();

            // Expected sums: Project 1 -> 4.0 + 2.5 = 6.5 ; Project 2 -> 3.5
            Assert.Equal([1, 2], projectHoursList.Select(kvp => kvp.Key).ToList());
            Assert.Equal(6.5m, projectHoursList.First(k => k.Key == 1).Value);
            Assert.Equal(3.5m, projectHoursList.First(k => k.Key == 2).Value);
        }

        [Fact]
        public void GetReportModel_WithInvalidIsoWeekString_ThrowsArgumentException()
        {
            // Arrange
            var mockRepo = new Mock<ITimesheetRepository>();
            mockRepo.Setup(r => r.GetAllUserIds()).Returns([]);
            mockRepo.Setup(r => r.GetAllIsoWeeks()).Returns([]);

            var service = new TimesheetReportingService(mockRepo.Object);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => service.GetReportModel(1, "invalid-format"));
        }
    }
}