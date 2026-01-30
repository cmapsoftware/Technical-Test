using Moq;
using System.ComponentModel.DataAnnotations;
using Timesheet.Models.Domain;
using Timesheet.Models.Dto;
using Timesheet.Repositories;
using Timesheet.Services;
using Timesheet.Tests.TestData;
using Timesheet.Validation;

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

        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryDto>(), null))
           .Returns(ValidationResult.Success);

        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

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
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

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

    #endregion Add

    #region Delete

    [Fact]
    public void Delete_ReturnsTrueWhenDeleted()
    {
        // Arrange
        var idToDelete = 1;

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(r => r.Delete(It.IsAny<int>()))
            .Returns(true);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

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
        var idToDelete = 1;

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(r => r.Delete(It.IsAny<int>()))
            .Returns(false);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

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

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(r => r.Update(It.IsAny<TimesheetEntry>()))
            .Returns(updatedEntity);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

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
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        expectedTimesheetEntry.Date = new DateTime(DateTime.Now.Year + 1, 1, 1); // set to future date
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

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

        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        expectedTimesheetEntry.Hours = 0.1M; // work hours below minimum of 0.25
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act

        // Assert
        var ex = Assert.Throws<ValidationException>(() =>
        {
            timesheetServiceMock.Add(timesheetEntryInsertDto);
        });
        Assert.Contains("Hours must be between 0.25 and 12.", ex.Message);
    }

    [Fact]
    public void Validate_MaximumWorkHours()
    {
        // Arrange        
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        expectedTimesheetEntry.Hours = 13M; // work hours above maximum of 12
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        //mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act

        // Assert
        var ex = Assert.Throws<ValidationException>(() =>
        {
            timesheetServiceMock.Add(timesheetEntryInsertDto);
        });
        Assert.Contains("Hours must be between 0.25 and 12.", ex.Message);
    }

    [Fact]
    public void Validate_MaximumDescriptionLength()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        expectedTimesheetEntry.Description = new string('A', 110); // description length above maximum of 100
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        //mock repo
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act

        // Assert
        var ex = Assert.Throws<ValidationException>(() =>
        {
            timesheetServiceMock.Add(timesheetEntryInsertDto);
        });
        Assert.Contains("Description cannot exceed 100 characters.", ex.Message);
    }

    [Fact]
    public void Validate_MinimumProjectId()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        expectedTimesheetEntry.ProjectId = 0; // invalid ProjectId below minimum of 1
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        //mock repo
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act

        // Assert
        var ex = Assert.Throws<ValidationException>(() =>
        {
            timesheetServiceMock.Add(timesheetEntryInsertDto);
        });
        Assert.Contains("ProjectId must be a positive integer.", ex.Message);
    }

    [Fact]
    public void Validate_MinimumUserId()
    {
        // Arrange
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        expectedTimesheetEntry.UserId = 0; // invalid UserId below minimum of 1
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        //mock repo
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(ValidationResult.Success);

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);


        // Act

        // Assert
        var ex = Assert.Throws<ValidationException>(() =>
        {
            timesheetServiceMock.Add(timesheetEntryInsertDto);
        });
        Assert.Contains("UserId must be a positive integer.", ex.Message);
    }

    [Fact]
    public void Add_PreventsDuplicateEntries()
    {
        // Arrange
        var expectedTimesheetEntry = TimesheetEntryData.ListTimesheetEntry[0];
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = expectedTimesheetEntry.UserId,
            ProjectId = expectedTimesheetEntry.ProjectId,
            Date = expectedTimesheetEntry.Date,
            Hours = expectedTimesheetEntry.Hours,
            Description = expectedTimesheetEntry.Description
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<TimesheetEntryInsert>()))
            .Returns(expectedTimesheetEntry);

        // mock validator (to simulate duplicate entry)
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
           .Setup(timesheetRepository => timesheetRepository.ValidateUnique(It.IsAny<TimesheetEntryInsertDto>(), null))
           .Returns(new ValidationResult("validation mesage"));

        // mock service
        var timesheetServiceMock = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        // Assert
        var ex = Assert.Throws<ValidationException>(() =>
        {
            timesheetServiceMock.Add(timesheetEntryInsertDto);
        });
        Assert.Contains("validation mesage", ex.Message);
    }

    #endregion Validation

    #region Validation tests (copilot generated)

    [Fact]
    public void Validate_ValidInsertDto_DoesNotThrow()
    {
        // Arrange
        var service = CreateService();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 1.0M,
            Description = "Normal description"
        };

        // Act
        var ex = Record.Exception(() => service.Validate(dto));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_InsertDto_WithFutureDate_ThrowsValidationException()
    {
        // Arrange
        var service = CreateService();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today.AddDays(1), // future
            Hours = 1.0M,
            Description = "Future date"
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => service.Validate(dto));
        Assert.Contains("Date cannot be in the future.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithTooSmallHours_ThrowsValidationException()
    {
        // Arrange
        var service = CreateService();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 0.1M, // below 0.25
            Description = "Too small hours"
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => service.Validate(dto));
        Assert.Contains("Hours must be between 0.25 and 12.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithTooLargeHours_ThrowsValidationException()
    {
        // Arrange
        var service = CreateService();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 13M, // above 12
            Description = "Too large hours"
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => service.Validate(dto));
        Assert.Contains("Hours must be between 0.25 and 12.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithTooLongDescription_ThrowsValidationException()
    {
        // Arrange
        var service = CreateService();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 1M,
            Description = new string('A', 110) // > 100
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => service.Validate(dto));
        Assert.Contains("Description cannot exceed 100 characters.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithInvalidUserId_ThrowsValidationException()
    {
        // Arrange
        var service = CreateService();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 0, // invalid
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 1M,
            Description = "Invalid user"
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => service.Validate(dto));
        Assert.Contains("UserId must be a positive integer.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithInvalidProjectId_ThrowsValidationException()
    {
        // Arrange
        var service = CreateService();
        var dto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 0, // invalid
            Date = DateTime.Today,
            Hours = 1M,
            Description = "Invalid project"
        };

        // Act & Assert
        var ex = Assert.Throws<ValidationException>(() => service.Validate(dto));
        Assert.Contains("ProjectId must be a positive integer.", ex.Message);
    }

    #endregion Validation tests  (copilot generated)

    #region GetDuplicateTimesheetRow tests (copilot generated)

    [Fact]
    public void GetDuplicateTimesheetRow_ReturnsNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        var repoMock = new Mock<ITimesheetRepository>();
        var validatorMock = new Mock<ITimesheetEntryValidator>();

        var userId = 1;
        var projectId = 2;
        var date = new DateTime(2024, 1, 1);
        int? currentRowId = null;

        repoMock
            .Setup(r => r.GetDuplicateTimesheetRow(userId, projectId, date, currentRowId))
            .Returns((TimesheetEntry?)null);

        var service = new TimesheetService(repoMock.Object, validatorMock.Object);

        // Act
        var result = service.GetDuplicateTimesheetRow(userId, projectId, date, currentRowId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetDuplicateTimesheetRow_ReturnsMappedDto_WhenRepositoryReturnsEntry()
    {
        // Arrange
        var repoMock = new Mock<ITimesheetRepository>();
        var validatorMock = new Mock<ITimesheetEntryValidator>();

        var expected = TimesheetEntryData.ListTimesheetEntry[0];

        repoMock
            .Setup(r => r.GetDuplicateTimesheetRow(expected.UserId, expected.ProjectId, expected.Date, It.IsAny<int?>()))
            .Returns(expected);

        var service = new TimesheetService(repoMock.Object, validatorMock.Object);

        // Act
        var result = service.GetDuplicateTimesheetRow(expected.UserId, expected.ProjectId, expected.Date, null);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expected.Id, result!.Id);
        Assert.Equal(expected.UserId, result.UserId);
        Assert.Equal(expected.ProjectId, result.ProjectId);
        Assert.Equal(expected.Date, result.Date);
        Assert.Equal(expected.Hours, result.Hours);
        Assert.Equal(expected.Description, result.Description);
    }

    #endregion GetDuplicateTimesheetRow tests  (copilot generated)

    private static TimesheetService CreateService()
    {
        var repoMock = new Mock<ITimesheetRepository>();
        var validatorMock = new Mock<ITimesheetEntryValidator>();
        return new TimesheetService(repoMock.Object, validatorMock.Object);
    }
}
