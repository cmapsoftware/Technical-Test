using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Timesheet.Models.Dto;
using Timesheet.Models.ViewModels;
using Timesheet.Services;

namespace Timesheet.Controllers;

// Note: Controller only does orchestration, no business logic
public class HomeController(ITimesheetService service) : Controller
{
    private readonly ITimesheetService _service = service;

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectWithInsertDto(null);
    }

    [HttpPost]
    public IActionResult Add([Bind(Prefix = "NewTimesheetEntry")] TimesheetEntryInsertDto timesheetEntryInsertDto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Message"] = "Add Timesheet entry failed.";
            TempData["MessageType"] = "alert-danger";
            return RedirectWithInsertDto(timesheetEntryInsertDto);
        }

        try
        {
            var timesheetEntryDto = _service.Add(timesheetEntryInsertDto);
            TempData["Message"] = $"Timesheet entry added with ID #{timesheetEntryDto?.Id}";
            TempData["MessageType"] = "alert-success";
            return RedirectToAction("Index");
        }
        catch (ValidationException ex)
        {
            TempData["Message"] = ex.Message;
            TempData["MessageType"] = "alert-danger";
            return RedirectWithInsertDto(timesheetEntryInsertDto);
        }
    }

    [HttpPost]
    public IActionResult RowAction(TimesheetEntryDto timesheetEntryDto, string action)
    {
        var id = timesheetEntryDto.Id;
        if (action == "update")
        {
            try
            {
                _service.Validate(timesheetEntryDto);
            }
            catch (ValidationException ex)
            {
                TempData["Message"] = $"Update failed for timesheet entry #{timesheetEntryDto.Id}. {ex.Message} Row has been reset.";
                TempData["MessageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            try
            {
                _service.Update(timesheetEntryDto);
                TempData["Message"] = $"Timesheet entry #{id} updated successfully.";
                TempData["MessageType"] = "alert-success";
            }
            catch (ValidationException ex)
            {
                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "alert-danger";
                return RedirectWithInsertDto(null);
            }
        }
        else if (action == "delete")
        {
            _service.Delete(id);
            TempData["Message"] = $"Timesheet entry #{id} deleted successfully.";
            TempData["MessageType"] = "alert-success";
        }

        return RedirectToAction("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private ViewResult RedirectWithInsertDto(TimesheetEntryInsertDto? timesheetEntryInsertDto)
    {
        timesheetEntryInsertDto ??= new TimesheetEntryInsertDto
        {
            Date = DateTime.Today
        };

        var timesheetEntryList = _service.GetAll();
        var homeViewModel = new HomepageViewModel
        {
            TimesheetEntries = [.. timesheetEntryList],
            NewTimesheetEntry = timesheetEntryInsertDto
        };
        return View("Index", homeViewModel);
    }
}