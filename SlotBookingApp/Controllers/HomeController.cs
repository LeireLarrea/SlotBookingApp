using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SlotBookingApp.Models;
using SlotBookingApp.Services;
using System.Diagnostics;

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
            date = "2024-07-09";
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
    public async Task<IActionResult> BookEvent([FromBody] CalendarEventModel eventData)
    {
        var validator = new CalendarEventModelValidator();
        var validationResult = await validator.ValidateAsync(eventData);
        if (!validationResult.IsValid)
        {
            var viewModelErrors = new BookEventViewModel
            {
                confirmationSlot = eventData.Start,
                Errors = validationResult.Errors
            };
            return View("BookEvent", viewModelErrors);
        }

        var bookingRequestResponse = await _bookingHelper.SendSlotBooking( eventData);

        var viewModel = new BookEventViewModel
        {
            confirmationName = bookingRequestResponse.Name,
            confirmationSlot = bookingRequestResponse.Slot,
            confirmationStatus = bookingRequestResponse.Status
        };

        return View("BookEvent", viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
