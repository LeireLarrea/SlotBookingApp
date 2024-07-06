using FluentValidation;

namespace SlotBookingApp.Models;

public class CalendarEventModelValidator : AbstractValidator<CalendarEventModel>
{
    public CalendarEventModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Please fill the Name")
            .NotNull().WithMessage("Name cannot be null")
            .Matches("^[a-zA-Z\\s]*$").WithMessage("NAME can contain only letters and spaces");

        RuleFor(x => x.SecondName)
           .NotEmpty().WithMessage("Please fill the SecondName")
           .NotNull().WithMessage("SecondName cannot be null")
           .Matches("^[a-zA-Z\\s]*$").WithMessage("SECOND NAME can contain only letters and spaces");

        RuleFor(x => x.Email)
           .NotEmpty().WithMessage("Please fill the Email")
           .NotNull().WithMessage("Email cannot be null")
           .EmailAddress().WithMessage("Invalid EMAIL format");

        RuleFor(x => x.Phone)
           .NotEmpty().WithMessage("Please fill the Phone")
           .NotNull().WithMessage("Phone cannot be null")
           .Must(BeAValidPhoneNumber).WithMessage("You PHONE NUMBER must be between 11 - 15 numbers long");

        RuleFor(x => x.Comments)
          .NotEmpty().WithMessage("Please fill the Comments")
          .NotNull().WithMessage("Comments cannot be null")
          .Matches("^[a-zA-Z\\s.,0-9]*$").WithMessage("COMMENTS can contain only letters, numbers, periods and commas");
    }

    private bool BeAValidPhoneNumber(string phoneNumber)
    {
        return phoneNumber.All(char.IsDigit) && phoneNumber.Length >= 11 && phoneNumber.Length <= 15;
    }
}
