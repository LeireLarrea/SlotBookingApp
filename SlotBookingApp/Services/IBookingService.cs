using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Models;

namespace SlotBookingApp.Services;

public interface IBookingService
{
    Task<PostBookingConfirmation> SendSlotBooking(CalendarEventModel eventData);
}
