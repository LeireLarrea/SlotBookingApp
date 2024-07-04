using SlotBookingApp.Models;

namespace SlotBookingApp.Services;

public interface IBookingService
{
    Task<bool> SendSlotBooking(CalendarEventModel eventData);
}
