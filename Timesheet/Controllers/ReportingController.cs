using Microsoft.AspNetCore.Mvc;
using Timesheet.Services;

namespace Timesheet.Controllers;

public class ReportingController(ITimesheetReportingService service) : Controller
{
    private readonly ITimesheetReportingService _service = service;

    [HttpGet]
    public IActionResult Index()
    {
        return View("Index", _service.GetReportModel(null, null));
    }

    [HttpPost]
    public IActionResult SubmitReport(int selectedUserId, string selectedIsoWeekIdIsoYearId)
    {
        return View("Index", _service.GetReportModel(selectedUserId, selectedIsoWeekIdIsoYearId));
    }
}