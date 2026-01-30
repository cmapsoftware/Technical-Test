using Moq;
using System.ComponentModel.DataAnnotations;
using Timesheet.Models.Domain;
using Timesheet.Models.Dto;
using Timesheet.Repositories;
using Timesheet.Services;
using Timesheet.Tests.TestData;

namespace Timesheet.Tests.Services;

public class TimesheetServiceTests
{
    #region Get

    [Fact]
    public void GetAll_ReturnsMappedDtos()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(timesheetRepository => timesheetRepository.GetAll())
            .Returns(TimesheetEntryData.ListTimesheetEntry);

        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object);

        // Act
        var timesheetEntryDtoList = timesheetServiceMock.GetAll().ToList();

        // Assert
        Assert.Equal(TimesheetEntryData.ListTimesheetEntry.Count, timesheetEntryDtoList.Count);

        for (int i = 0; i < timesheetEntryDtoList.Count; i++)
        {
            var expected = TimesheetEntryData.ListTimesheetEntry[i];
            var actual = timesheetEntryDtoList[i];

            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.ProjectId, actual.ProjectId);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.Hours, actual.Hours);
            Assert.Equal(expected.Description, actual.Description);
        }
    }

    #endregion Get

    #region Add    

    [Fact]
    public void Add_ReturnsDto()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];

        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object);

        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        // Act
        var timesheetEntryDto = timesheetServiceMock.Add(timesheetEntryInsertDto);

        // Assert
        Assert.NotNull(timesheetEntryDto);
        Assert.Equal(expectedTimesheetEntry.Id, timesheetEntryDto.Id);
        Assert.Equal(expectedTimesheetEntry.UserId, timesheetEntryDto.UserId);
        Assert.Equal(expectedTimesheetEntry.ProjectId, timesheetEntryDto.ProjectId);
        Assert.Equal(expectedTimesheetEntry.Date, timesheetEntryDto.Date);
        Assert.Equal(expectedTimesheetEntry.Hours, timesheetEntryDto.Hours);
        Assert.Equal(expectedTimesheetEntry.Description, timesheetEntryDto.Description);
    }

    [Fact]
    public void Add_PreventsDuplicateEntries() //TODO: plus others needed!
    {

    }

    #endregion Add

    #region Delete

    [Fact]
    public void Delete_ReturnsTrueWhenDeleted()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();

        // The repository should return true when the item exists and is deleted
        timesheetRepositoryMock
            .Setup(r => r.Delete(It.IsAny<int>()))
            .Returns(true);

        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object);
        var idToDelete = 1;

        // Act
        var result = timesheetServiceMock.Delete(1);

        // Assert
        Assert.True(result);
        timesheetRepositoryMock.Verify(r => r.Delete(idToDelete), Times.Once);
    }

    [Fact]
    public void Delete_ReturnsFalseWhenIdNotPresent()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();

        // The repository should return true when the item exists and is deleted
        timesheetRepositoryMock
            .Setup(r => r.Delete(It.IsAny<int>()))
            .Returns(false);

        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object);
        var idToDelete = 1;

        // Act
        var result = timesheetServiceMock.Delete(1);

        // Assert
        Assert.False(result);
        timesheetRepositoryMock.Verify(r => r.Delete(idToDelete), Times.Once);
    }

    #endregion Delete

    #region Update

    [Fact]
    public void Update_ReturnsUpdatedDto()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        var existingTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];

        var updatedEntity = new TimesheetEntry
        {
            Id = existingTimesheetEntry.Id,
            UserId = existingTimesheetEntry.UserId,
            ProjectId = existingTimesheetEntry.ProjectId,
            Date = existingTimesheetEntry.Date,
            Hours = existingTimesheetEntry.Hours + 1, // updated property
            Description = "Updated description"       // updated property
        };

        timesheetRepositoryMock
            .Setup(r => r.Update(It.IsAny<TimesheetEntry>()))
            .Returns(updatedEntity);

        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object);

        var updateDto = new TimesheetEntryDto
        {
            Id = updatedEntity.Id,
            UserId = updatedEntity.UserId,
            ProjectId = updatedEntity.ProjectId,
            Date = updatedEntity.Date,
            Hours = updatedEntity.Hours,
            Description = updatedEntity.Description
        };

        // Act
        var updatedTimeshettEntryDto = timesheetServiceMock.Update(updateDto);

        // Assert
        Assert.NotNull(updatedTimeshettEntryDto);
        Assert.Equal(updatedEntity.Id, updatedTimeshettEntryDto.Id);
        Assert.Equal(updatedEntity.UserId, updatedTimeshettEntryDto.UserId);
        Assert.Equal(updatedEntity.ProjectId, updatedTimeshettEntryDto.ProjectId);
        Assert.Equal(updatedEntity.Date, updatedTimeshettEntryDto.Date);
        Assert.Equal(updatedEntity.Hours, updatedTimeshettEntryDto.Hours);
        Assert.Equal(updatedEntity.Description, updatedTimeshettEntryDto.Description);

        timesheetRepositoryMock.Verify(r => r.Update(It.IsAny<TimesheetEntry>()), Times.Once);

    }

    #endregion Update

    #region Validation

    // Note: Validation should run for Add and Update, but here is only tested for Add for brevity

    [Fact]
    public void Validate_RejectFutureDates()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        expectedTimesheetEntry.Date = new DateTime(DateTime.Now.Year + 1, 1, 1); // set to future date
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object);
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        // Act

        // Assert
        var ex = Assert.Throws<ValidationException>(() =>
        {
            timesheetServiceMock.Add(timesheetEntryInsertDto);
        });

        Assert.Contains("Date cannot be in the future.", ex.Message);
    }

    [Fact]
    public void Validate_MinimumWorkHours()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        expectedTimesheetEntry.Hours = 0.1M; // work hours below minimum
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object);
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        // Act

        // Assert
        var ex = Assert.Throws<ValidationException>(() =>
        {
            timesheetServiceMock.Add(timesheetEntryInsertDto);
        });
        Assert.Contains("Hours must be between 0.25 and 12.", ex.Message);
    }

    [Fact]
    public void Validate_MaxmumWorkHours()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        expectedTimesheetEntry.Hours = 13M; // work hours above maximum
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object);
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        // Act

        // Assert
        var ex = Assert.Throws<ValidationException>(() =>
        {
            timesheetServiceMock.Add(timesheetEntryInsertDto);
        });
        Assert.Contains("Hours must be between 0.25 and 12.", ex.Message);
    }

    #endregion Validation
}