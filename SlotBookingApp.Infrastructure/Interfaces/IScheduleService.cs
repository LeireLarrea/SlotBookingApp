using SlotBookingApp.Domain.Entities;
using SlotBookingApp.Infrastructure.Dtos;

namespace SlotBookingApp.Infrastructure.Interfaces;

public interface IScheduleService
{
    Task<List<CalendarEvent>> GetAvailableSlots(ScheduleData scheduleData, string date);
    Task<ScheduleData> GetSchedule(string date);
}
