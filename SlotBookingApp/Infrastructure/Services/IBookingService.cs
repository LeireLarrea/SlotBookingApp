using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Models;

namespace SlotBookingApp.Infrastructure.Services;

public interface IBookingService
{
    Task<PostBookingConfirmationDto> SendSlotBooking(CalendarEventModel eventData);
}
