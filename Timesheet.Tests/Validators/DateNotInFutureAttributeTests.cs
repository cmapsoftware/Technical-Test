using Timesheet.Validation;

namespace Timesheet.Tests.Validators;

public class DateNotInFutureAttributeTests
{
    #region DateNotInFutureAttribute Tests (copilot generated)

    [Fact]
    public void IsValid_PastDate_ReturnsTrue()
    {
        // Arrange
        var attribute = new DateNotInFutureAttribute();
        var pastDate = DateTime.Today.AddDays(-1);

        // Act
        var result = attribute.IsValid(pastDate);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_TodayDate_ReturnsTrue()
    {
        // Arrange
        var attribute = new DateNotInFutureAttribute();
        var today = DateTime.Today;

        // Act
        var result = attribute.IsValid(today);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_FutureDate_ReturnsFalse()
    {
        // Arrange
        var attribute = new DateNotInFutureAttribute();
        var futureDate = DateTime.Today.AddDays(1);

        // Act
        var result = attribute.IsValid(futureDate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_NullValue_ReturnsTrue()
    {
        // Arrange
        var attribute = new DateNotInFutureAttribute();

        // Act
        var result = attribute.IsValid(null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_NonDateTimeValue_ReturnsTrue()
    {
        // Arrange
        var attribute = new DateNotInFutureAttribute();
        var nonDate = "2026-01-01";

        // Act
        var result = attribute.IsValid(nonDate);

        // Assert
        Assert.True(result);
    }
    #endregion DateNotInFutureAttribute Tests (copilot generated)
}