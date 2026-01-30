using System.ComponentModel.DataAnnotations;
using Timesheet.Models.Domain;
using Timesheet.Models.Dto;
using Timesheet.Repositories;
using Timesheet.Validation;

namespace Timesheet.Services;

public class TimesheetService(ITimesheetRepository timesheetRepository, ITimesheetEntryValidator timesheetEntryValidator) : ITimesheetService
{
    private readonly ITimesheetRepository _timesheetRepository = timesheetRepository;
    private readonly ITimesheetEntryValidator _timesheetEntryValidator = timesheetEntryValidator;

    public IEnumerable<TimesheetEntryDto> GetAll()
    {
        var timesheetEntries = _timesheetRepository.GetAll();
        var timesheetEntryDtos = timesheetEntries.Select(e => new TimesheetEntryDto
        {
            Id = e.Id,
            UserId = e.UserId,
            ProjectId = e.ProjectId,
            Date = e.Date,
            Hours = e.Hours,
            Description = e.Description
        });
        return timesheetEntryDtos;
    }

    // Note: this method is very specific, in a full project I'd do something more general
    public TimesheetEntryDto? GetDuplicateTimesheetRow(int userId, int projectId, DateTime date, int? currentUserId)
    {
        var timesheetEntry = _timesheetRepository.GetDuplicateTimesheetRow( userId, projectId, date, currentUserId);

        if (timesheetEntry == null)
        {
            return null;
        }

        return new TimesheetEntryDto
        {
            Id = timesheetEntry.Id,
            UserId = timesheetEntry.UserId,
            ProjectId = timesheetEntry.ProjectId,
            Date = timesheetEntry.Date,
            Hours = timesheetEntry.Hours,
            Description = timesheetEntry.Description
        };
    }

    public TimesheetEntryDto? Add(TimesheetEntryInsertDto timesheetEntryInsertDto)
    {
        Validate(timesheetEntryInsertDto);

        var validationResult = _timesheetEntryValidator.ValidateUnique(timesheetEntryInsertDto);
        if (validationResult != ValidationResult.Success)
        {
            throw new ValidationException(validationResult!.ErrorMessage);
        }

        var timesheetEntry = _timesheetRepository.Add(new TimesheetEntryInsert
        {
            UserId = timesheetEntryInsertDto.UserId,
            ProjectId = timesheetEntryInsertDto.ProjectId,
            Date = timesheetEntryInsertDto.Date,
            Hours = timesheetEntryInsertDto.Hours,
            Description = timesheetEntryInsertDto.Description
        });

        return timesheetEntry == null ? null : new TimesheetEntryDto
        {
            Id = timesheetEntry.Id,
            UserId = timesheetEntry.UserId,
            ProjectId = timesheetEntry.ProjectId,
            Date = timesheetEntry.Date,
            Hours = timesheetEntry.Hours,
            Description = timesheetEntry.Description
        };
    }

    public TimesheetEntryDto? Update(TimesheetEntryDto timesheetEntryDto)
    {
        Validate(timesheetEntryDto);

        var validationResult = _timesheetEntryValidator.ValidateUnique(timesheetEntryDto, timesheetEntryDto.Id);
        if (validationResult != ValidationResult.Success)
        {
            throw new ValidationException(validationResult!.ErrorMessage);
        }

        var timesheetEntryUpdate = new TimesheetEntry
        {
            Id = timesheetEntryDto.Id,
            UserId = timesheetEntryDto.UserId,
            ProjectId = timesheetEntryDto.ProjectId,
            Date = timesheetEntryDto.Date,
            Hours = timesheetEntryDto.Hours,
            Description = timesheetEntryDto.Description
        };

        var timesheetEntry = _timesheetRepository.Update(timesheetEntryUpdate);

        return timesheetEntry == null ? null : new TimesheetEntryDto
        {
            Id = timesheetEntry.Id,
            UserId = timesheetEntry.UserId,
            ProjectId = timesheetEntry.ProjectId,
            Date = timesheetEntry.Date,
            Hours = timesheetEntry.Hours,
            Description = timesheetEntry.Description
        };
    }

    public bool Delete(int id)
    {
        return _timesheetRepository.Delete(id);
    }

    // this will force validation of the DTO using its data annotations
    // (i.e. validate not called by MVC pipeline, such as in unit tests)
    public void Validate(object dto)
    {
        var validationContext = new ValidationContext(dto);
        Validator.ValidateObject(dto, validationContext, validateAllProperties: true);
    }
}