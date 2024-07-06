using SlotBookingApp.Helpers;
using SlotBookingApp.Infrastructure.Dtos;


namespace SlotBookingApp.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly ILogger<ScheduleService> _logger;
    private readonly DateHelper _dateHelper;
    private readonly SlotsHelper _slotsHelper;
    private readonly HttpClientHelper _httpClientHelper;
    private readonly string _getWeeklyAvailabilitypathURL = "availability/GetWeeklyAvailability/";


    public ScheduleService(DateHelper dateHelper, SlotsHelper slotsHelper,
        ILogger<ScheduleService> logger, HttpClientHelper httpClientHelper)
    {
        _dateHelper = dateHelper;
        _slotsHelper = slotsHelper;
        _logger = logger;
        _httpClientHelper = httpClientHelper;
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
            var httpClient = _httpClientHelper.GetClient();
            var scheduleDataRespose = await httpClient.GetAsync($"{_getWeeklyAvailabilitypathURL}{_dateHelper.GetWeeksMonday(date).ToString("yyyyMMdd")}");
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