using Newtonsoft.Json;
using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SlotBookingApp.Services;

public class BookingService : IBookingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<BookingService> _logger;
    private readonly IConfiguration _configuration;

    public BookingService(IHttpClientFactory httpClientFactory, ILogger<BookingService> logger, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
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

    public async Task<bool> SendSlotBooking(CalendarEventModel eventData)
    {
        var slotBookingDto = await CreateBookingFromCalendarEvent(eventData);
        bool isSent = await PostSlotBooking(slotBookingDto);
        return isSent;      // TODO: improve response -> include statuscode, and evendata.Name, bookingtime
    }

    private async Task<bool> PostSlotBooking(SlotBookingDto slotBookingDto)
    {
        _logger.LogInformation($"PostSlotBooking call STARTED for {slotBookingDto.Start}");

        try
        {
            var json = JsonConvert.SerializeObject(slotBookingDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient("ExternalApi");
            var authHeaderValue = GetAuthHeaderValue();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

            var response = await client.PostAsync("availability/TakeSlot", content);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation($"PostSlotBooking call COMPLETED for {slotBookingDto.Start}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"PostSlotBooking call ERROR: {ex.Message}");
            return false;
        }
    }

    private string GetAuthHeaderValue()
    {
        var username = _configuration["ApiCredentials:Username"];
        var password = _configuration["ApiCredentials:Password"];
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    }
}
