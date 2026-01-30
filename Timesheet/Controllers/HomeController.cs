using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Timesheet.Models.Dto;
using Timesheet.Models.ViewModels;
using Timesheet.Services;

namespace Timesheet.Controllers;

public class HomeController(ITimesheetService service) : Controller
{
    private readonly ITimesheetService _service = service;

    [HttpGet]
    public IActionResult Index()
    {
        var timesheetEntryList = _service.GetAll();
        var homeViewModel = new HomepageViewModel
        {
            TimesheetEntries = [.. timesheetEntryList],
            NewTimesheetEntry = new TimesheetEntryInsertDto
            {
                Date = DateTime.Today
            }
        };
        return View(homeViewModel);
    }

    [HttpPost]
    public IActionResult Add([Bind(Prefix = "NewTimesheetEntry")] TimesheetEntryInsertDto timesheetEntryInsertDto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Message"] = "Add Timesheet entry failed.";
            TempData["MessageType"] = "alert-danger";
            var timesheetEntryList = _service.GetAll();
            return View("Index", new HomepageViewModel
            {
                TimesheetEntries = [.. timesheetEntryList],
                NewTimesheetEntry = timesheetEntryInsertDto
            });
        }

        var timesheetEntryDto = _service.Add(timesheetEntryInsertDto);
        TempData["Message"] = $"Timesheet entry added with ID #{timesheetEntryDto?.Id}";
        TempData["MessageType"] = "alert-success";
        return RedirectToAction("Index");
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
                //return View("Index", GetViewModel()); // or however you reload the view
                return RedirectToAction("Index");
            }

            _service.Update(timesheetEntryDto);
            TempData["Message"] = $"Timesheet entry #{id} updated successfully.";
            TempData["MessageType"] = "alert-success";
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
}