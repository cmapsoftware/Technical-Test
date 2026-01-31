using Moq;
using System.ComponentModel.DataAnnotations;
using Timesheet.Models.Domain;
using Timesheet.Models.Dto;
using Timesheet.Repositories;
using Timesheet.Validation;

namespace Timesheet.Tests.Validators;

public class TimesheetEntryValidatorTests
{
    #region ValidateUnique Tests (copilot generated

    [Fact]
    public void ValidateUnique_ReturnsSuccess_WhenNoDuplicateFound()
    {
        // Arrange
        var repoMock = new Mock<ITimesheetRepository>();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 2,
            Date = new DateTime(2024, 1, 1),
            Hours = 8m,
            Description = "Work"
        };

        repoMock
            .Setup(r => r.GetDuplicateTimesheetRow(dto.UserId, dto.ProjectId, dto.Date, null))
            .Returns((TimesheetEntry?)null);

        var validator = new TimesheetEntryValidator(repoMock.Object);

        // Act
        var result = validator.ValidateUnique(dto);

        // Assert
        Assert.Equal(ValidationResult.Success, result);
    }

    [Fact]
    public void ValidateUnique_ReturnsValidationResult_WithInsertMessage_WhenDuplicateExists()
    {
        // Arrange
        var repoMock = new Mock<ITimesheetRepository>();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 10,
            ProjectId = 20,
            Date = new DateTime(2024, 2, 2),
            Hours = 7.5m,
            Description = "Duplicate test"
        };

        repoMock
            .Setup(r => r.GetDuplicateTimesheetRow(dto.UserId, dto.ProjectId, dto.Date, null))
            .Returns(new TimesheetEntry { Id = 5, UserId = dto.UserId, ProjectId = dto.ProjectId, Date = dto.Date, Hours = dto.Hours, Description = dto.Description });

        var validator = new TimesheetEntryValidator(repoMock.Object);

        // Act
        var result = validator.ValidateUnique(dto);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(ValidationResult.Success, result);
        var expectedMessage = $"A timesheet entry already exists with UserId={dto.UserId}, ProjectId={dto.ProjectId} and Date={DateOnly.FromDateTime(dto.Date)}. Insert failed";
        Assert.Equal(expectedMessage, result!.ErrorMessage);
    }

    [Fact]
    public void ValidateUnique_ReturnsValidationResult_WithUpdateMessage_WhenDuplicateExistsAndRowIdProvided()
    {
        // Arrange
        var repoMock = new Mock<ITimesheetRepository>();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 3,
            ProjectId = 4,
            Date = new DateTime(2024, 3, 3),
            Hours = 6m,
            Description = "Update duplicate test"
        };

        int rowId = 42;

        repoMock
            .Setup(r => r.GetDuplicateTimesheetRow(dto.UserId, dto.ProjectId, dto.Date, rowId))
            .Returns(new TimesheetEntry { Id = 9, UserId = dto.UserId, ProjectId = dto.ProjectId, Date = dto.Date, Hours = dto.Hours, Description = dto.Description });

        var validator = new TimesheetEntryValidator(repoMock.Object);

        // Act
        var result = validator.ValidateUnique(dto, rowId);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(ValidationResult.Success, result);
        var expectedMessage = $"A timesheet entry already exists with UserId={dto.UserId}, ProjectId={dto.ProjectId} and Date={DateOnly.FromDateTime(dto.Date)}. Update failed for Row ID #{rowId}.";
        Assert.Equal(expectedMessage, result!.ErrorMessage);
    }

    #endregion ValidateUnique Tests
}