using Newtonsoft.Json;
using SlotBookingApp.Helpers;
using SlotBookingApp.Infrastructure.Dtos;
using System.Net.Http.Headers;


namespace SlotBookingApp.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _client;

    private readonly DateHelper _dateHelper;
    private readonly SlotsHelper _slotsHelper;
    private readonly HttpHelper _httpHelper;
    private readonly string _getWeeklyAvailabilitypathURL = "availability/GetWeeklyAvailability/";

    public ScheduleService(ILogger<ScheduleService> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, 
        DateHelper dateHelper, SlotsHelper slotsHelper, HttpHelper httpHelper)
    {
        _logger = logger;
        _configuration = configuration;

        _httpClientFactory = httpClientFactory;
        _client = _httpClientFactory.CreateClient();
        _client.BaseAddress = new Uri(_configuration["DraliatestSettings:BaseUrl"]);

        _dateHelper = dateHelper;
        _slotsHelper = slotsHelper;
        _httpHelper = httpHelper;
        var authHeaderValue = _httpHelper.GetAuthHeaderValue();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
    }

    /// <summary>
    /// Retrieves the weekly schedule data based on any provided date for such week.
    /// </summary>
    /// <param name="date">The date for which the weekly schedule is requested.</param>
    /// <returns>The schedule data for the specified week.</returns>
    public async Task<ScheduleData> GetSchedule(string date)
    {
        _logger.LogInformation($"GetSchedule call STARTED for date: {date}");

        try
        {
            var scheduleDataRespose = await _client.GetAsync($"{_getWeeklyAvailabilitypathURL}{_dateHelper.GetWeeksMonday(date).ToString("yyyyMMdd")}");
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

    /// <summary>
    /// Calculates the available slots based on the provided schedule data and date.
    /// </summary>
    /// <param name="scheduleData">The schedule data for the week.</param>
    /// <param name="date">The day for which the weekly available slots are requested.</param>
    /// <returns>A list of calendar events representing available slots formatted for FullCalendar.</returns>
    public async Task<List<object>> GetAvailableSlots(ScheduleData scheduleData, string date)
    {
        List<string> allSlots = await _slotsHelper.GetAllSlots(scheduleData, date);
        List<string> busySlots = await _slotsHelper.GetBusySlots(scheduleData);
        List<string> availableSlots = allSlots.Except(busySlots).ToList();
        var calendarEvents = _slotsHelper.FormatCalendarEventsForFullCalendar(availableSlots);
        return calendarEvents;
    }
}