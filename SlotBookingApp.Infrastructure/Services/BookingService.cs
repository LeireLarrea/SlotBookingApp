using AutoMapper;
using Newtonsoft.Json;
using SlotBookingApp.Domain.Entities;
using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Infrastructure.Interfaces;
using System.Net.Http.Headers;
using System.Text;

public class BookingService : IBookingService
{
    private readonly IMapper _mapper;
    private readonly IHttpClientFactory _httpClientFactory;

    public BookingService(IMapper mapper, IHttpClientFactory httpClientFactory)
    {
        _mapper = mapper;
        _httpClientFactory = httpClientFactory;
    }

    private async Task<SlotBookingDto> CreateBookingFromCalendarEvent(CalendarEvent calendarEvent)
    {
        //var slotBookingDto = _mapper.Map<SlotBookingDto>(calendarEvent); // TODO:

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

    public async Task<bool> SendSlotBooking(CalendarEvent eventData)
    {
        var slotBookingDto = await CreateBookingFromCalendarEvent(eventData);
        bool isSent = await PostSlotBooking(slotBookingDto);
        return isSent;      // TODO: improve response -> include statuscode, and evendata.Name, bookingtime
    }

    private async Task<bool> PostSlotBooking(SlotBookingDto slotBookingDto)
    {
        try
        {
            var json = JsonConvert.SerializeObject(slotBookingDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient("ExternalApi");
            var authHeaderValue = GetAuthHeaderValue();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);

            var response = await client.PostAsync("availability/TakeSlot", content);
            response.EnsureSuccessStatusCode();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private string GetAuthHeaderValue()
    {
        var username = "techuser";          // TODO: get credentials from config
        var password = "secretpassWord";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    }
}
