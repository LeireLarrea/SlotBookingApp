using Microsoft.AspNetCore.Mvc;
using SlotBookingApp.Models;
using System.Diagnostics;
using SlotBookingApp.Domain.Entities;
using SlotBookingApp.Infrastructure.Interfaces;
using Newtonsoft.Json.Linq;


namespace SlotBookingApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IScheduleService _scheduleService;
    private readonly IBookingService _bookingHelper;

    public HomeController(ILogger<HomeController> logger, IScheduleService scheduleService, IBookingService bookingHelper)
    {
        _logger = logger;
        _scheduleService = scheduleService;
        _bookingHelper = bookingHelper;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAvailableSlots(string date)
    {
        try
        {
            var scheduleData = await _scheduleService.GetSchedule(date);
            var availableSlots = await _scheduleService.GetAvailableSlots(scheduleData, date);
            
            var response = new
            {
                FacilityId = scheduleData.Facility.FacilityId.ToString(), 
                Events = availableSlots
            };
            return Json(response); 
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    [HttpPost]
    public IActionResult BookEvent([FromBody] CalendarEvent eventData)
    {
        try
        {
            var isSent = _bookingHelper.SendSlotBooking( eventData);

            return Ok(new { message = "Event booked successfully!" });
        }
        catch (Exception ex)
        {
            // Log the error or handle it appropriately
            return StatusCode(500, new { message = $"Error: {ex.Message}" });
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
