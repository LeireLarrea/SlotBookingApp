using SlotBookingApp.Domain.Entities;

namespace SlotBookingApp.Infrastructure.Interfaces;

public interface IBookingService
{
    Task<List<string>> SendSlotBooking(CalendarEvent eventData);
}
