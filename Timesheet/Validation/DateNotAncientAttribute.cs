using System.ComponentModel.DataAnnotations;

namespace Timesheet.Validation;

public class DateNotAncientAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is DateTime date)
        {
            return date.Date >= new DateTime(2000, 01, 01); // reasonable assumption, must be in this century!
        }

        // If value is null or not a DateTime, consider it valid.
        // Other validation attributes should handle these checks.
        // Prevent multiple error messages for the same issue.
        return true;
    }
}
