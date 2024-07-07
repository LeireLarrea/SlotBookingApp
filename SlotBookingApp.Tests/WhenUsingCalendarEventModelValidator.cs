using FluentAssertions;
using SlotBookingApp.Models;

namespace SlotBookingApp.Tests;

public class WhenUsingCalendarEventModelValidator
{
    private readonly CalendarEventModelValidator _validator;

    public WhenUsingCalendarEventModelValidator()
    {
        _validator = new CalendarEventModelValidator(); 
    }

    [Theory]
    [InlineData("Test Name", "Test Surname", "test@email.com", "12345678901", "Doctors help me", true, 0)]
    [InlineData("TestName", "TestSurname", "test@email.com", "12345678901", "Doctors help me", true, 0)]
    [InlineData("", "TestSurname", "test@email.com", "12345678901", "Doctors help me", false, 1)]
    [InlineData("Test-Name", "TestSurname", "test@email.com", "12345678901", "Doctors help me", false, 1)]
    [InlineData("Test4Name", "TestSurname", "test@email.com", "12345678901", "Doctors help me", false, 1)]
    [InlineData("TestName", "", "test@email.com", "12345678901", "Doctors help me", false, 1)]
    [InlineData("TestName", "Test'Surname", "test@email.com", "12345678901", "Doctors help me", false, 1)]
    [InlineData("TestName", "Test Surname", "", "12345678901", "Doctors help me", false, 1)]
    [InlineData("TestName", "Test Surname", "testemail.com", "12345678901", "Doctors help me", false, 1)]
    [InlineData("TestName", "TestSurname", "test@email.com", "123456781", "Doctors help me", false, 1)]
    [InlineData("TestName", "TestSurname", "test@email.com", "12345678901", "", false, 1)]
    [InlineData("TestName", "TestSurname", "test@email.com", "12345678901", "testing sql'injection", false, 1)]
    [InlineData("", "", "", "", "", false, 5)]

    public void BeAValidPhoneNumber_Should_Return_Correct_Validation(string name, string secondName, string email, string phoneNumber, string comments, bool shouldBeValid, int expectedErrorCount)
    {
        // Act
        var result = _validator.Validate(new CalendarEventModel {Name = name, SecondName = secondName, Email = email,  Phone = phoneNumber, Comments = comments });

        // Assert
        result.IsValid.Should().Be(shouldBeValid);
        result.Errors.Should().HaveCount(expectedErrorCount);
    }
}
