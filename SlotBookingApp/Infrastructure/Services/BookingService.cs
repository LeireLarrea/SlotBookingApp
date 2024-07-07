using Newtonsoft.Json;
using SlotBookingApp.Helpers;
using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Models;
using System.Net.Http.Headers;
using System.Text;

namespace SlotBookingApp.Infrastructure.Services;

public class BookingService : IBookingService
{
    private readonly ILogger<BookingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _client;
    private readonly HttpHelper _httpHelper;


    public BookingService(ILogger<BookingService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, HttpHelper httpHelper)
    {
        _logger = logger;

        _configuration = configuration;

        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient();
        _client.BaseAddress = new Uri(_configuration["DraliatestSettings:BaseUrl"]);

        _httpHelper = httpHelper;
        var authHeaderValue = _httpHelper.GetAuthHeaderValue();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
    }


    public async Task<SlotBookingDto> CreateBookingFromCalendarEvent(CalendarEventModel calendarEvent)
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

    /// <summary>
    /// POSTS a slot booking request using the provided event data.
    /// </summary>
    /// <param name="eventData">The calendar event data for slot booking.</param>
    /// <returns>A PostBookingConfirmationDto indicating is the http code of the POST request.</returns>

    public async Task<PostBookingConfirmationDto> SendSlotBooking(CalendarEventModel eventData)
    {
        var slotBookingDto = await CreateBookingFromCalendarEvent(eventData);
        int statusCode = await PostSlotBooking(slotBookingDto);

        var confirmation = new PostBookingConfirmationDto { Name = eventData.Name, Slot = eventData.Start, Status = statusCode.ToString() };
        return confirmation;
    }

    private async Task<int> PostSlotBooking(SlotBookingDto slotBookingDto)
    {
        _logger.LogInformation($"PostSlotBooking call STARTED for {slotBookingDto.Start}");

        try
        {
            var json = JsonConvert.SerializeObject(slotBookingDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("availability/TakeSlot", content);
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
