using Timesheet.Validation;

namespace Timesheet.Tests.Validators;

public class DateNotAncientAttributeTests
{
    #region DateNotAncientAttribute Tests (copilot generated)

    [Fact]
    public void IsValid_DateAfter2000_ReturnsTrue()
    {
        // Arrange
        var attribute = new DateNotAncientAttribute();
        var date = new DateTime(2023, 6, 15);

        // Act
        var result = attribute.IsValid(date);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_Exactly2000Jan1_ReturnsTrue()
    {
        // Arrange
        var attribute = new DateNotAncientAttribute();
        var date = new DateTime(2000, 1, 1);

        // Act
        var result = attribute.IsValid(date);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_Before2000_ReturnsFalse()
    {
        // Arrange
        var attribute = new DateNotAncientAttribute();
        var ancientDate = new DateTime(1999, 12, 31);

        // Act
        var result = attribute.IsValid(ancientDate);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_Before2000_WithTimePortion_ReturnsFalse()
    {
        // Arrange
        var attribute = new DateNotAncientAttribute();
        var ancientDateWithTime = new DateTime(1999, 12, 31, 23, 59, 59);

        // Act
        var result = attribute.IsValid(ancientDateWithTime);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_NullValue_ReturnsTrue()
    {
        // Arrange
        var attribute = new DateNotAncientAttribute();

        // Act
        var result = attribute.IsValid(null);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_NonDateTimeValue_ReturnsTrue()
    {
        // Arrange
        var attribute = new DateNotAncientAttribute();
        var nonDate = "1999-12-31";

        // Act
        var result = attribute.IsValid(nonDate);

        // Assert
        Assert.True(result);
    }

    #endregion DateNotAncientAttribute Tests (copilot generated)
}