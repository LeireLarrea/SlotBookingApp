using SlotBookingApp.Models;

namespace SlotBookingApp.Services;

public interface IBookingService
{
    Task<ConfirmationViewModel> SendSlotBooking(CalendarEventModel eventData);
}
