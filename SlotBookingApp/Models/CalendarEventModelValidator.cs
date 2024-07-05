using FluentValidation;
namespace SlotBookingApp.Models;

public class CalendarEventModelValidator : AbstractValidator<CalendarEventModel>
{
    public CalendarEventModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Please fill the Name");

        RuleFor(x => x.SecondName)
           .NotEmpty().WithMessage("Please fill the SecondName")
           .NotNull().WithMessage("SecondName cannot be null");

        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Please fill the Email")
           .NotNull().WithMessage("Email cannot be null")
           .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Phone)
           .NotEmpty().WithMessage("Please fill the Phone")
           .NotNull().WithMessage("Phone cannot be null")
           .Must(BeAValidPhoneNumber).WithMessage("Invalid mobile-phone number format.");

        RuleFor(x => x.Comments)
          .NotEmpty().WithMessage("Please fill the Comments")
          .NotNull().WithMessage("Comments cannot be null");
    }

    private bool BeAValidPhoneNumber(string phoneNumber)
    {
        return phoneNumber.All(char.IsDigit) && phoneNumber.Length >= 11 && phoneNumber.Length <= 15;
    }
}
