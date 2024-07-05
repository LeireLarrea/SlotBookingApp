namespace SlotBookingApp.Models;

public class BookEventViewModel
{
    public string confirmationName { get; set; }
    public string confirmationSlot { get; set; }
    public string confirmationStatus {  get; set; }
    public List<FluentValidation.Results.ValidationFailure> Errors { get; set; }

}
