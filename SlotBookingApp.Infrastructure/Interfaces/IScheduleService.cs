using SlotBookingApp.Infrastructure.Dtos;

namespace SlotBookingApp.Infrastructure.Interfaces;

public interface IScheduleService
{
    Task<ScheduleData> GetSchedule(string date);
}
