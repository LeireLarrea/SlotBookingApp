using System.Net.Http.Headers;
using System.Text;
using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Infrastructure.Helpers;
using System.Net.Http.Json;
using SlotBookingApp.Infrastructure.Interfaces;

namespace SlotBookingApp.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly HttpClient _httpClient;
    private readonly DateHelper _dateHelper;

    public ScheduleService(IHttpClientFactory httpClientFactory, DateHelper dateHelper)
    {
        _httpClient = httpClientFactory.CreateClient("ExternalApi");
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes("techuser:secretpassWord"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        _dateHelper = dateHelper;
    }

    public async Task<ScheduleData> GetSchedule(string date)
    {
        var scheduleDataRespose = await _httpClient.GetAsync($"availability/GetWeeklyAvailability/{_dateHelper.GetWeeksMonday(date)}");
        scheduleDataRespose.EnsureSuccessStatusCode();

        var weeklySchedule = await scheduleDataRespose.Content.ReadFromJsonAsync<ScheduleData>();
        return weeklySchedule;
    }
}
