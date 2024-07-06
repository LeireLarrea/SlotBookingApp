using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using SlotBookingApp.Infrastructure.Services;
using SlotBookingApp.Models;


namespace SlotBookingApp.Controllers;

/// <summary>
/// Controller for handling calendar page and slot booking operations.
/// </summary>
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

    /// <summary>
    /// Retrieves available slots for the specified week based on a date of that week
    /// </summary>
    /// <param name="date">The date for which slots are requested.</param>
    /// <returns>JSON response containing available slots.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAvailableSlots(string date)
    {
        _logger.LogInformation($"GetAvailableSlots call STARTED for date {date}");

        try
        {
            var scheduleData = await _scheduleService.GetSchedule(date);
            var availableSlots = await _scheduleService.GetAvailableSlots(scheduleData, date);
            
            var response = new
            {
                FacilityId = scheduleData.Facility.FacilityId.ToString(), 
                Events = availableSlots
            };

            _logger.LogInformation($"GetAvailableSlots call COMPLETED for date {date}");

            return Json(response); 
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"GetAvailableSlots call ERROR for date {date}: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }

    /// <summary>
    /// Books an event/slot using the selected event data.
    /// </summary>
    /// <param name="eventData">The event data to book.</param>
    /// <returns>Action result indicating success or failure of the booking operation.</returns>
    [HttpPost]
    public async Task<IActionResult> BookEvent([FromBody] CalendarEventModel eventData)
    {
        _logger.LogInformation($"BookEvent call STARTED for slot {eventData.Start}");

        var validator = new CalendarEventModelValidator();
        var validationResult = await validator.ValidateAsync(eventData);

        if (!validationResult.IsValid)
        {
            _logger.LogInformation($"BookEvent call VALIDATION ERROR/S for slot {eventData.Start}");
            return BadRequest(validationResult.Errors);
        }

        var bookingRequestResponse = await _bookingHelper.SendSlotBooking(eventData);
        if (bookingRequestResponse.Status == "500")
        {
            _logger.LogInformation($"BookEvent call SERVER ERROR for slot {eventData.Start}");
            var serverError = new FluentValidation.Results.ValidationResult() { 
                Errors = new List<FluentValidation.Results.ValidationFailure> { new ValidationFailure("", "Server Error. Please try again later") }
            };
            return BadRequest(serverError);
        }

        _logger.LogInformation($"BookEvent call COMPLETED for slot {eventData.Start}");
        return Ok(bookingRequestResponse);
    }
}
