using Newtonsoft.Json;
using SlotBookingApp.Helpers;
using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Models;
using System.Text;

namespace SlotBookingApp.Services;

public class BookingService : IBookingService
{
    private readonly ILogger<BookingService> _logger;
    private readonly HttpClientHelper _httpClientHelper;

    public BookingService(ILogger<BookingService> logger, HttpClientHelper httpClientHelper)
    {
        _logger = logger;
        _httpClientHelper = httpClientHelper;
    }


    private async Task<SlotBookingDto> CreateBookingFromCalendarEvent(CalendarEventModel calendarEvent)
    {
        var calendarEventJson = JsonConvert.SerializeObject(calendarEvent);
        SlotBookingDto slotBooking = JsonConvert.DeserializeObject<SlotBookingDto>(calendarEventJson);

        slotBooking.Patient = new PatientDto
        {
            Name = calendarEvent.Name,
            SecondName = calendarEvent.SecondName,
            Email = calendarEvent.Email,
            Phone = calendarEvent.Phone
        };

        return slotBooking;
    }

    public async Task<PostBookingConfirmation> SendSlotBooking(CalendarEventModel eventData)
    {
        var slotBookingDto = await CreateBookingFromCalendarEvent(eventData);
        int statusCode = await PostSlotBooking(slotBookingDto);

        var confirmation = new PostBookingConfirmation { Name = eventData.Name, Slot = eventData.Start, Status = statusCode.ToString()};
        return confirmation;     
    }

    private async Task<int> PostSlotBooking(SlotBookingDto slotBookingDto)
    {
        _logger.LogInformation($"PostSlotBooking call STARTED for {slotBookingDto.Start}");

        try
        {
            var json = JsonConvert.SerializeObject(slotBookingDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpClient = _httpClientHelper.GetClient();
            var response = await httpClient.PostAsync("availability/TakeSlot", content);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation($"PostSlotBooking call COMPLETED for {slotBookingDto.Start}");

            return (int)response.StatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError($"PostSlotBooking call ERROR: {ex.Message}");
            return 500;
        }
    }
}
