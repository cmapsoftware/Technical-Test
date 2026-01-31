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

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        var timesheetEntryDtoList = timesheetService.GetAll().ToList();

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

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        var timesheetEntryDto = timesheetService.Add(timesheetEntryInsertDto);

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

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        var result = timesheetService.Delete(1);

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

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        var result = timesheetService.Delete(1);

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

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

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
        var updatedTimesheetEntryDto = timesheetService.Update(updateDto);

        // Assert
        Assert.NotNull(updatedTimesheetEntryDto);
        Assert.Equal(updatedEntity.Id, updatedTimesheetEntryDto.Id);
        Assert.Equal(updatedEntity.UserId, updatedTimesheetEntryDto.UserId);
        Assert.Equal(updatedEntity.ProjectId, updatedTimesheetEntryDto.ProjectId);
        Assert.Equal(updatedEntity.Date, updatedTimesheetEntryDto.Date);
        Assert.Equal(updatedEntity.Hours, updatedTimesheetEntryDto.Hours);
        Assert.Equal(updatedEntity.Description, updatedTimesheetEntryDto.Description);

        timesheetRepositoryMock.Verify(r => r.Update(It.IsAny<TimesheetEntry>()), Times.Once);
    }

    [Fact]
    public void Update_ThrowsValidationException_WhenDtoInvalid()
    {
        // Arrange
        var timesheetEntryDto = TimesheetEntryData.TimesheetEntryDto;
        timesheetEntryDto.UserId = 0; // invalid UserId

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        // Arrange
        Assert.Throws<ValidationException>(() => timesheetService.Update(timesheetEntryDto));
    }

    [Fact]
    public void Update_ThrowsValidationException_WhenUniqueValidationFails()
    {
        // Arrange
        var timesheetEntryDto = TimesheetEntryData.TimesheetEntryDto;
        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
            .Setup(v => v.ValidateUnique(timesheetEntryDto, timesheetEntryDto.Id))
            .Returns(new ValidationResult("Duplicate entry"));

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        // Assert
        var ex = Assert.Throws<ValidationException>(() => timesheetService.Update(timesheetEntryDto));
        Assert.Contains("Duplicate entry", ex.Message);

        timesheetRepositoryMock.Verify(r => r.Update(It.IsAny<TimesheetEntry>()), Times.Never);
    }

    [Fact]
    public void Update_ReturnsNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        var timesheetEntryDto = TimesheetEntryData.TimesheetEntryDto;

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(r => r.Update(It.IsAny<TimesheetEntry>()))
            .Returns((TimesheetEntry?)null);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
            .Setup(v => v.ValidateUnique(TimesheetEntryData.TimesheetEntryDto, TimesheetEntryData.TimesheetEntryDto.Id))
            .Returns(ValidationResult.Success);

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        var result = timesheetService.Update(timesheetEntryDto);

        // Assert
        Assert.Null(result);
        timesheetRepositoryMock.Verify(r => r.Update(It.Is<TimesheetEntry>(t =>
            t.Id == timesheetEntryDto.Id &&
            t.UserId == timesheetEntryDto.UserId &&
            t.ProjectId == timesheetEntryDto.ProjectId &&
            t.Date == timesheetEntryDto.Date &&
            t.Hours == timesheetEntryDto.Hours &&
            t.Description == timesheetEntryDto.Description)), Times.Once);
    }

    [Fact]
    public void Update_ReturnsMappedDto_WhenRepositoryReturnsEntity()
    {
        // Arrange
        var timesheetEntryDto = TimesheetEntryData.TimesheetEntryDto;

        var returnedTimesheetEntry = new TimesheetEntry
        {
            Id = timesheetEntryDto.Id,
            UserId = timesheetEntryDto.UserId,
            ProjectId = timesheetEntryDto.ProjectId,
            Date = timesheetEntryDto.Date,
            Hours = timesheetEntryDto.Hours,
            Description = timesheetEntryDto.Description
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        timesheetRepositoryMock
            .Setup(r => r.Update(It.IsAny<TimesheetEntry>()))
            .Returns(returnedTimesheetEntry);

        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();
        timesheetEntryValidatorMock
            .Setup(v => v.ValidateUnique(timesheetEntryDto, timesheetEntryDto.Id))
            .Returns(ValidationResult.Success);

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        var result = timesheetService.Update(timesheetEntryDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(timesheetEntryDto.Id, result!.Id);
        Assert.Equal(timesheetEntryDto.UserId, result.UserId);
        Assert.Equal(timesheetEntryDto.ProjectId, result.ProjectId);
        Assert.Equal(timesheetEntryDto.Date, result.Date);
        Assert.Equal(timesheetEntryDto.Hours, result.Hours);
        Assert.Equal(timesheetEntryDto.Description, result.Description);
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
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 1.0M,
            Description = "Normal description"
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        var ex = Record.Exception(() => timesheetService.Validate(timesheetEntryInsertDto));

        // Assert
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_InsertDto_WithFutureDate_ThrowsValidationException()
    {
        // Arrange
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today.AddDays(1), // future
            Hours = 1.0M,
            Description = "Future date"
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);


        // Act
        // Assert
        var ex = Assert.Throws<ValidationException>(() => timesheetService.Validate(timesheetEntryInsertDto));
        Assert.Contains("Date cannot be in the future.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithTooSmallHours_ThrowsValidationException()
    {
        // Arrange
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 0.1M, // below 0.25
            Description = "Too small hours"
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        // Assert
        var ex = Assert.Throws<ValidationException>(() => timesheetService.Validate(timesheetEntryInsertDto));
        Assert.Contains("Hours must be between 0.25 and 12.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithTooLargeHours_ThrowsValidationException()
    {
        // Arrange
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 13M, // above 12
            Description = "Too large hours"
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        // Assert
        var ex = Assert.Throws<ValidationException>(() => timesheetService.Validate(timesheetEntryInsertDto));
        Assert.Contains("Hours must be between 0.25 and 12.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithTooLongDescription_ThrowsValidationException()
    {
        // Arrange
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 1M,
            Description = new string('A', 110) // > 100
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        // Assert
        var ex = Assert.Throws<ValidationException>(() => timesheetService.Validate(timesheetEntryInsertDto));
        Assert.Contains("Description cannot exceed 100 characters.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithInvalidUserId_ThrowsValidationException()
    {
        // Arrange
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = 0, // invalid
            ProjectId = 1,
            Date = DateTime.Today,
            Hours = 1M,
            Description = "Invalid user"
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        // Assert
        var ex = Assert.Throws<ValidationException>(() => timesheetService.Validate(timesheetEntryInsertDto));
        Assert.Contains("UserId must be a positive integer.", ex.Message);
    }

    [Fact]
    public void Validate_InsertDto_WithInvalidProjectId_ThrowsValidationException()
    {
        // Arrange
        var timesheetEntryInsertDto = new TimesheetEntryInsertDto
        {
            UserId = 1,
            ProjectId = 0, // invalid
            Date = DateTime.Today,
            Hours = 1M,
            Description = "Invalid project"
        };

        // mock repo
        var timesheetRepositoryMock = new Mock<ITimesheetRepository>();
        // mock validator
        var timesheetEntryValidatorMock = new Mock<ITimesheetEntryValidator>();

        var timesheetService = new TimesheetService(timesheetRepositoryMock.Object, timesheetEntryValidatorMock.Object);

        // Act
        // Assert
        var ex = Assert.Throws<ValidationException>(() => timesheetService.Validate(timesheetEntryInsertDto));
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
}
