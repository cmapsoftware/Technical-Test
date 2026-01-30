using FluentAssertions;
using Timesheet.Repositories;
using Timesheet.Tests.TestData;

namespace Timesheet.Tests.Repositories;

public class TimesheetRepositoryTests
{
    #region Get

    [Fact]
    public void GetAll_ShouldReturnEmptyListInitially()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();

        // Act
        var timeSheetEntryList = timesheetRepository.GetAll();

        // Assert
        Assert.Empty(timeSheetEntryList);
    }

    [Fact]
    public void GetAll_ShouldReturnAllInsertedItems()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();

        var timesheetEntryInsert1 = TimesheetEntryData.ListTimesheetEntryInsert[0];
        var timesheetEntryInsert2 = TimesheetEntryData.ListTimesheetEntryInsert[1];
        var timesheetEntryInsert3 = TimesheetEntryData.ListTimesheetEntryInsert[2];

        // Act
        var timesheetEntry1WithId = timesheetRepository.Add(timesheetEntryInsert1);
        var timesheetEntry2WithId = timesheetRepository.Add(timesheetEntryInsert2);
        var timesheetEntry3WithId = timesheetRepository.Add(timesheetEntryInsert3);

        // Assert
        var timesheetEntryList = timesheetRepository.GetAll();
        Assert.Contains(timesheetEntry1WithId, timesheetEntryList);
        Assert.Contains(timesheetEntry2WithId, timesheetEntryList);
        Assert.Contains(timesheetEntry3WithId, timesheetEntryList);
    }

    #endregion Get  <!-- I like annotating the #endregion. It makes large files easier to navigate. -->

    #region Add

    [Fact]
    public void Add_ShouldStoreEntityInRepository()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();
        var timesheetEntry = TimesheetEntryData.ListTimesheetEntryInsert[0];

        // Act
        // get copy of the entry with assigned Id
        var timesheetEntryWithId = timesheetRepository.Add(timesheetEntry);

        // Assert
        Assert.Contains(timesheetEntryWithId, timesheetRepository.GetAll());
    }

    [Fact]
    public void Add_ShouldIncrementEntryIds()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();
        var timesheetEntry1 = TimesheetEntryData.ListTimesheetEntryInsert[0];
        var timesheetEntry2 = TimesheetEntryData.ListTimesheetEntryInsert[1];
        var timesheetEntry3 = TimesheetEntryData.ListTimesheetEntryInsert[2];

        // Act
        var timesheetEntry1WithId = timesheetRepository.Add(timesheetEntry1);
        var timesheetEntry2WithId = timesheetRepository.Add(timesheetEntry2);
        var timesheetEntry3WithId = timesheetRepository.Add(timesheetEntry3);

        // Assert
        Assert.Equal(1, timesheetEntry1WithId.Id);
        Assert.Equal(2, timesheetEntry2WithId.Id);
        Assert.Equal(3, timesheetEntry3WithId.Id);
    }

    [Fact]
    public void Add_ShouldAllowEmptyDescription()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();
        var timesheetEntryWithNoDescription = TimesheetEntryData.ListTimesheetEntryInsert[2];

        // Act
        // get copy of the entry with assigned Id
        var timesheetEntrytimesheetEntryWithNoDescriptionWithId = timesheetRepository.Add(timesheetEntryWithNoDescription);

        // Assert
        Assert.Contains(timesheetEntrytimesheetEntryWithNoDescriptionWithId, timesheetRepository.GetAll());
        Assert.Null(timesheetEntrytimesheetEntryWithNoDescriptionWithId.Description);
    }

    [Fact]
    public void Add_ShouldNotReuseIdsOfDeletedEntries()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();
        var timesheetEntryToDelete1 = TimesheetEntryData.ListTimesheetEntryInsert[0];
        var timesheetEntryToDelete2 = TimesheetEntryData.ListTimesheetEntryInsert[1];
        var timesheetEntryToInsertAfterDeletions = TimesheetEntryData.ListTimesheetEntryInsert[2];

        // add timesheet entries
        var timesheetEntryToDelete1Id = timesheetRepository.Add(timesheetEntryToDelete1).Id;
        var timesheetEntryToDelete2Id = timesheetRepository.Add(timesheetEntryToDelete2).Id;

        // delete timesheet entries
        timesheetRepository.Delete(timesheetEntryToDelete1Id);
        timesheetRepository.Delete(timesheetEntryToDelete1Id);

        // Act
        var timesheetEntryToInsertAfterDeletionsId = timesheetRepository.Add(timesheetEntryToInsertAfterDeletions).Id;

        // Assert
        var timeSheetEntryList = timesheetRepository.GetAll();
        Assert.DoesNotContain(timeSheetEntryList, timesheetEntry => timesheetEntry.Id == timesheetEntryToDelete1Id);
        Assert.DoesNotContain(timeSheetEntryList, timesheetEntry => timesheetEntry.Id == timesheetEntryToDelete1Id);
        Assert.Contains(timeSheetEntryList, timesheetEntry => timesheetEntry.Id == timesheetEntryToInsertAfterDeletionsId);

        Assert.Equal(1, timesheetEntryToDelete1Id);
        Assert.Equal(2, timesheetEntryToDelete2Id);
        Assert.Equal(3, timesheetEntryToInsertAfterDeletionsId);
    }

    #endregion Add 

    #region Delete

    [Fact]
    public void Delete_ShouldReturnTrueWhenEntityDeleted()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();
        var timesheetEntryInsert = TimesheetEntryData.ListTimesheetEntryInsert[0];
        var timesheetEntry1WithId = timesheetRepository.Add(timesheetEntryInsert);

        // Act
        var result = timesheetRepository.Delete(timesheetEntry1WithId.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Delete_ShouldReturnFalseWhenEntityNotDeleted()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();
        var failingDeletionId = 0; // any integer will fail as repository is empty

        // Act
        var result = timesheetRepository.Delete(failingDeletionId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Delete_ShouldRemoveOnlyCorrectEntityFromRepository()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();
        var timesheetEntryToBeDeleted = TimesheetEntryData.ListTimesheetEntryInsert[0];
        var timesheetEntryToRemainUnaffected = TimesheetEntryData.ListTimesheetEntryInsert[1];
        var timesheetEntryToBeDeletedWithId = timesheetRepository.Add(timesheetEntryToBeDeleted);
        var timesheetEntryToRemainUnaffectedWithId = timesheetRepository.Add(timesheetEntryToRemainUnaffected);

        // Act
        timesheetRepository.Delete(timesheetEntryToBeDeletedWithId.Id);

        // Assert
        var timeSheetEntryList = timesheetRepository.GetAll();
        Assert.DoesNotContain(timesheetEntryToBeDeletedWithId, timeSheetEntryList);
        Assert.Contains(timesheetEntryToRemainUnaffectedWithId, timeSheetEntryList);
    }

    #endregion Delete

    #region Update

    [Fact]
    public void Update_ShouldUpdateOnlyCorrectEntityInRepository()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();
        var timesheetEntryUnaffected = TimesheetEntryData.ListTimesheetEntryInsert[0];
        var timesheetEntryToBeUpdated = TimesheetEntryData.ListTimesheetEntryInsert[1];
        var timesheetEntryUpdater = TimesheetEntryData.ListTimesheetEntry[2];
        timesheetRepository.Add(timesheetEntryUnaffected);
        var timesheetEntryToBeUpdatedWithId =  timesheetRepository.Add(timesheetEntryToBeUpdated);

        // Make timesheetEntryUpdater have the same Id as timesheetEntryToBeUpdated (all other properties differ)
        timesheetEntryUpdater.Id = timesheetEntryToBeUpdatedWithId.Id;

        // Act
        var timesheetEntryUpdated = timesheetRepository.Update(timesheetEntryUpdater);

        // Assert
        var timeSheetEntryList = timesheetRepository.GetAll();

        // FluentAssertions performs a property-based comparison rather than reference equality, which we need here
        timeSheetEntryList.Should().ContainEquivalentOf(timesheetEntryUnaffected);
        timeSheetEntryList.Should().ContainEquivalentOf(timesheetEntryUpdated);
        timeSheetEntryList.Should().NotContainEquivalentOf(timesheetEntryToBeUpdated);
    }


    [Fact]
    public void Update_ShouldReturnNullIfEntryNotFound()
    {
        // Arrange
        var timesheetRepository = new TimesheetRepository();
        var timesheetEntry = TimesheetEntryData.ListTimesheetEntryInsert[0];
        var timesheetEntryUpdaterNotInRepository = TimesheetEntryData.ListTimesheetEntry[1];

        timesheetRepository.Add(timesheetEntry);

        // Act
        var timesheetEntryUpdated = timesheetRepository.Update(timesheetEntryUpdaterNotInRepository);

        // Assert
        Assert.Null(timesheetEntryUpdated);
    }

    #endregion Update
}