using System.ComponentModel.DataAnnotations;

namespace Timesheet.Validation
{
public class DateNotInFutureAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime date)
            {
                return date.Date <= DateTime.Today;
            }

            // If value is null or not a DateTime, consider it valid.
            // Other validation attributes should handle these checks.
            // Prevent multiple error messages for the same issue.
            return true; 
        }
    }
}