using Microsoft.AspNetCore.Mvc;
using SlotBookingApp.Models;
using System.Diagnostics;
using SlotBookingApp.Domain.Entities;
using SlotBookingApp.Infrastructure.Interfaces;


namespace SlotBookingApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IScheduleService _scheduleService;

    public HomeController(ILogger<HomeController> logger, IScheduleService scheduleService)
    {
        _logger = logger;
        _scheduleService = scheduleService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<JsonResult> GetAvailableSlots(string date)
    {
        var scheduleData = await _scheduleService.GetSchedule(date);
        List<CalendarEvent> availableSlots = await _scheduleService.GetAvailableSlots(scheduleData, date);
        return Json(availableSlots);
    }

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
