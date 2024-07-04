using Newtonsoft.Json;
using SlotBookingApp.Helpers;
using SlotBookingApp.Infrastructure.Dtos;
using System.Net.Http.Headers;
using System.Text;


namespace SlotBookingApp.Services;

public class ScheduleService : IScheduleService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ScheduleService> _logger;
    private readonly DateHelper _dateHelper;
    private readonly SlotsHelper _slotsHelper;


    public ScheduleService(IHttpClientFactory httpClientFactory, DateHelper dateHelper, SlotsHelper slotsHelper, ILogger<ScheduleService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ExternalApi");
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes("techuser:secretpassWord"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        _dateHelper = dateHelper;
        _slotsHelper = slotsHelper;
        _logger = logger;
    }

    public async Task<ScheduleData> GetSchedule(string date)
    {
        _logger.LogInformation($"GetSchedule call STARTED for date: {date}");

        try
        {
            var scheduleDataRespose = await _httpClient.GetAsync($"availability/GetWeeklyAvailability/{_dateHelper.GetWeeksMonday(date).ToString("yyyyMMdd")}");
            scheduleDataRespose.EnsureSuccessStatusCode();

            var weeklySchedule = await scheduleDataRespose.Content.ReadFromJsonAsync<ScheduleData>();
            _logger.LogInformation($"GetSchedule call COMPLETED for date: {date}");
            
            return weeklySchedule;
        }
        catch (Exception ex)
        {
            _logger.LogError($"GetSchedule call ERROR: {ex.Message}");
            return new ScheduleData() { };
        }
    }

    public async Task<List<Object>> GetAvailableSlots(ScheduleData scheduleData, string date)
    {
        List<string> allSlots =  await _slotsHelper.GetAllSlots(scheduleData, date);
        List<string> busySlots = await _slotsHelper.GetBusySlots(scheduleData);
        List<string> availableSlots = allSlots.Except(busySlots).ToList();
        var testll = JsonConvert.SerializeObject(availableSlots);
        var calendarEvents = _slotsHelper.FormatCalendarEventsForFullCalendar(availableSlots);
        return calendarEvents;
    } 
}