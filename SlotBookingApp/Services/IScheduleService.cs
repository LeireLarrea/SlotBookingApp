using SlotBookingApp.Infrastructure.Dtos;

namespace SlotBookingApp.Services;

public interface IScheduleService
{
    Task<List<Object>> GetAvailableSlots(ScheduleData scheduleData, string date);
    Task<ScheduleData> GetSchedule(string date);
}
