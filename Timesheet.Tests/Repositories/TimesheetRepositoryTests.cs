using FluentAssertions;
using Timesheet.Models.Domain;
using Timesheet.Repositories;
using Timesheet.Tests.TestData;

namespace Timesheet.Tests.Repositories
{
    public class TimesheetRepositoryTests
    {
        #region GetAll

        [Fact]
        public void GetAll_ShouldReturnEmptyListInitially()
        {
            // Arrange
            var timesheetRepository = new TimesheetRepository();

            // Act
            var timeSheetEntryList = timesheetRepository.GetAllTimesheetEntries();

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
            var timesheetEntryList = timesheetRepository.GetAllTimesheetEntries();
            Assert.Contains(timesheetEntry1WithId, timesheetEntryList);
            Assert.Contains(timesheetEntry2WithId, timesheetEntryList);
            Assert.Contains(timesheetEntry3WithId, timesheetEntryList);
        }

        #endregion GetAll  <!-- I like annotating the #endregion. It makes large files easier to navigate. -->

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
            Assert.Contains(timesheetEntryWithId, timesheetRepository.GetAllTimesheetEntries());
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
            Assert.Contains(timesheetEntrytimesheetEntryWithNoDescriptionWithId, timesheetRepository.GetAllTimesheetEntries());
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
            var timeSheetEntryList = timesheetRepository.GetAllTimesheetEntries();
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
            var timeSheetEntryList = timesheetRepository.GetAllTimesheetEntries();
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
            var timesheetEntryToBeUpdatedWithId = timesheetRepository.Add(timesheetEntryToBeUpdated);

            // Make timesheetEntryUpdater have the same Id as timesheetEntryToBeUpdated (all other properties differ)
            timesheetEntryUpdater.Id = timesheetEntryToBeUpdatedWithId.Id;

            // Act
            var timesheetEntryUpdated = timesheetRepository.Update(timesheetEntryUpdater);

            // Assert
            var timeSheetEntryList = timesheetRepository.GetAllTimesheetEntries();

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

        #region GetDuplicate (copilot generated)

        [Fact]
        public void GetDuplicateTimesheetRow_ReturnsNull_WhenNoEntriesExist()
        {
            // Arrange
            var repo = new TimesheetRepository();
            var date = new DateTime(2024, 1, 1);

            // Act
            var result = repo.GetDuplicateTimesheetRow(userId: 1, projectId: 1, date: date, currentRowId: null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetDuplicateTimesheetRow_ReturnsEntry_WhenDuplicateExists()
        {
            // Arrange
            var repo = new TimesheetRepository();
            var date = new DateTime(2024, 1, 1);

            var inserted = repo.Add(new TimesheetEntryInsert
            {
                UserId = 10,
                ProjectId = 20,
                Date = date,
                Hours = 2.5M,
                Description = "Work"
            });

            // Act
            var result = repo.GetDuplicateTimesheetRow(userId: 10, projectId: 20, date: date, currentRowId: null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inserted.Id, result!.Id);
            Assert.Equal(inserted.UserId, result.UserId);
            Assert.Equal(inserted.ProjectId, result.ProjectId);
            Assert.Equal(inserted.Date, result.Date);
        }

        [Fact]
        public void GetDuplicateTimesheetRow_ExcludesCurrentRowId_WhenUpdating()
        {
            // Arrange
            var repo = new TimesheetRepository();
            var date = new DateTime(2024, 1, 1);

            var inserted = repo.Add(new TimesheetEntryInsert
            {
                UserId = 5,
                ProjectId = 6,
                Date = date,
                Hours = 1.0M,
                Description = "Desc"
            });

            // Act - when currentRowId equals the existing entry id, it should not be considered a duplicate
            var excluded = repo.GetDuplicateTimesheetRow(userId: inserted.UserId, projectId: inserted.ProjectId, date: inserted.Date, currentRowId: inserted.Id);

            // Assert
            Assert.Null(excluded);

            // Act - when currentRowId is different (or null) it should return the existing entry
            var found = repo.GetDuplicateTimesheetRow(userId: inserted.UserId, projectId: inserted.ProjectId, date: inserted.Date, currentRowId: inserted.Id + 1);

            // Assert
            Assert.NotNull(found);
            Assert.Equal(inserted.Id, found!.Id);
        }

        [Fact]
        public void GetDuplicateTimesheetRow_DoesNotReturn_WhenDifferentDateOrProjectOrUser()
        {
            // Arrange
            var repo = new TimesheetRepository();
            var date = new DateTime(2024, 1, 1);

            repo.Add(new TimesheetEntryInsert
            {
                UserId = 1,
                ProjectId = 1,
                Date = date,
                Hours = 1M,
                Description = "A"
            });

            // Act & Assert - different user
            var r1 = repo.GetDuplicateTimesheetRow(userId: 2, projectId: 1, date: date, currentRowId: null);
            Assert.Null(r1);

            // different project
            var r2 = repo.GetDuplicateTimesheetRow(userId: 1, projectId: 2, date: date, currentRowId: null);
            Assert.Null(r2);

            // different date
            var r3 = repo.GetDuplicateTimesheetRow(userId: 1, projectId: 1, date: date.AddDays(1), currentRowId: null);
            Assert.Null(r3);
        }

        #endregion GetDuplicate (copilot generated)
    }
}