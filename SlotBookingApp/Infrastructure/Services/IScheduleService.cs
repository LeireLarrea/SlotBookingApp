using SlotBookingApp.Infrastructure.Dtos;

namespace SlotBookingApp.Infrastructure.Services;

public interface IScheduleService
{
    Task<List<object>> GetAvailableSlots(ScheduleData scheduleData, string date);
    Task<ScheduleData> GetSchedule(string date);
}
