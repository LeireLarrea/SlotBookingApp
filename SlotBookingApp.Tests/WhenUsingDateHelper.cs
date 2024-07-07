using FluentAssertions;
using SlotBookingApp.Helpers;
using SlotBookingApp.Infrastructure.Dtos;
using System.Diagnostics.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace SlotBookingApp.Tests.Helpers;

[ExcludeFromCodeCoverage]
public class WhenUsingDateHelper    {

    [Theory]
    [InlineData("2024-07-08", "2024-07-08")]  // input monday returns monday
    [InlineData("2024-07-17", "2024-07-15")]  // input wednesday returns monday
    [InlineData("2024-07-28", "2024-07-22")]  // input sunday returns monday
    public void GetWeeksMonday_Should_ReturnWeeksMonday(string inputDateString, string expectedMondayDateString)
    {
        // Arrange
        var dateHelper = new DateHelper();

        // Act
        DateTime inputDate = DateTime.ParseExact(inputDateString, "yyyy-MM-dd", null);
        DateTime expectedMonday = DateTime.ParseExact(expectedMondayDateString, "yyyy-MM-dd", null);
        DateTime actualMonday = dateHelper.GetWeeksMonday(inputDateString);

        // Assert
        Assert.Equal(expectedMonday, actualMonday);
    }

    [Fact]
    public void GetWeeksMonday_Should_ThrowException_WhenInputInInvalidFormat()
    {
        // Arrange
        var dateHelper = new DateHelper();

        // Act & Assert
        Assert.Throws<FormatException>(() => dateHelper.GetWeeksMonday("01/02/2024"));
    }

    [Theory]
    [InlineData(30, 10, 3)]
    [InlineData(60, 15, 4)]
    [InlineData(120, 20, 6)]
    public void GenerateTimeList_Should_GeneratesCorrectTimeSlots(int timespam, int intervalMinutes, int expectedCount)
    {
        // Arrange
        var dateHelper = new DateHelper();
        DateTime start = DateTime.Now.AddMinutes(10); 
        DateTime end = start.AddMinutes(timespam);

        // Act
        List<string> timeSlots = dateHelper.GenerateTimeList(start, end, intervalMinutes);

        // Assert
        Assert.Equal(expectedCount, timeSlots.Count);
    }

    [Fact]
    public void GenerateTimeList_Should_ThrowException_WhenIntervalZeroOrNegative()
    {
        // Arrange
        var dateHelper = new DateHelper();
        DateTime start = DateTime.Now;
        DateTime end = DateTime.Now.AddMinutes(60);
        int intervalMinutes = 0;

        // Act 
        List<string> timeSlots = dateHelper.GenerateTimeList(start, end, intervalMinutes);

        // Assert
        timeSlots.Should().BeEquivalentTo(new List<string>());
    }
}
