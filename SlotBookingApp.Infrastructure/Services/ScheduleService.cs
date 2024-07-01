using System.Net.Http.Headers;
using System.Text;
using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Infrastructure.Helpers;
using System.Net.Http.Json;
using SlotBookingApp.Infrastructure.Interfaces;
using SlotBookingApp.Domain.Entities;
using System;


namespace SlotBookingApp.Infrastructure.Services;

public class ScheduleService : IScheduleService
{
    private readonly HttpClient _httpClient;
    private readonly DateHelper _dateHelper;
    private readonly SlotsHelper _slotsHelper;


    public ScheduleService(IHttpClientFactory httpClientFactory, DateHelper dateHelper, SlotsHelper slotsHelper)
    {
        _httpClient = httpClientFactory.CreateClient("ExternalApi");
        var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes("techuser:secretpassWord"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        _dateHelper = dateHelper;
        _slotsHelper = slotsHelper;
    }

    public async Task<ScheduleData> GetSchedule(string date)
    {
        var scheduleDataRespose = await _httpClient.GetAsync($"availability/GetWeeklyAvailability/{_dateHelper.GetWeeksMonday(date).ToString("yyyyMMdd")}");
        scheduleDataRespose.EnsureSuccessStatusCode();

        var weeklySchedule = await scheduleDataRespose.Content.ReadFromJsonAsync<ScheduleData>();
        return weeklySchedule;
    }

    public async Task<List<CalendarEvent>> GetAvailableSlots(ScheduleData scheduleData, string date)
    {
        List<string> allSlots =  await _slotsHelper.GetAllSlots(scheduleData, date);
        List<string> busySlots = await _slotsHelper.GetBusySlots(scheduleData);
        List<string> availableSlots = allSlots.Except(busySlots).ToList();
        //need to map availableslots to CalendarEvent
        //return await Task.FromResult(availableSlots);
        return new List<CalendarEvent>();
    } 
}