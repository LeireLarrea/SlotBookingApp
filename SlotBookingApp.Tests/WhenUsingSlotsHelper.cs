using FluentAssertions;
using FluentAssertions.Execution;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;

using SlotBookingApp.Helpers;
using SlotBookingApp.Infrastructure.Dtos;


namespace SlotBookingApp.Tests.Helpers;

[ExcludeFromCodeCoverage]
public class SlotsHelperTests
{
    [Fact]
    public async Task GetAllSlots_Should_ReturnsCorrectSlotsWhenValidScheduleData()
    {
        // Arrange
        var dateHelper = new DateHelper();
        var slotsHelper = new SlotsHelper(dateHelper);

        ScheduleData scheduleData = JsonConvert.DeserializeObject<ScheduleData>(validScheduleJson);

        string date = "2024-07-09"; 

        // Act
        List<string> allSlots = await slotsHelper.GetAllSlots(scheduleData, date);

        // Assert
        using(new AssertionScope())
        {
            Assert.NotEmpty(allSlots);
            Assert.Equal(allSlots.Count, 84);
            Assert.Contains("10/07/2024 11:00:00 - 10/07/2024 11:10:00", allSlots);
            Assert.Contains("08/07/2024 09:00:00 - 08/07/2024 09:10:00", allSlots);
            Assert.DoesNotContain("08/08/2024 09:00:00 - 08/08/2024 09:10:00", allSlots);
        }
    }

    [Fact]
    public async Task GetAllSlots_Should_ThrowExceptionWhenDataIsNull()
    {
        // Arrange
        var dateHelper = new DateHelper();
        var slotsHelper = new SlotsHelper(dateHelper);

        ScheduleData scheduleData = JsonConvert.DeserializeObject<ScheduleData>(inValidScheduleJson);
        string date = "2024-07-09";

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => slotsHelper.GetAllSlots(scheduleData, date));
    }

    [Fact]
    public async Task GetBusySlots_Should_Return_FormattedSlots_When_ValidData()
    {
        // Arrange
        var slotsHelper = new SlotsHelper(new DateHelper());
        var scheduleData = JsonConvert.DeserializeObject<ScheduleData>(validScheduleJson);

        // Act
        var result = await slotsHelper.GetBusySlots(scheduleData);

        // Assert
        using(new AssertionScope())
        {
            result.Should().NotBeEmpty();
            result.Should().HaveCount(5);
            result.Should().Contain("10/07/2024 14:00:00 - 10/07/2024 14:10:00");
            result.Should().NotContain("10/07/2024 14:10:00 - 10/07/2024 14:20:00");
        }
    }

    [Fact]
    public async Task GetBusySlots_Should_ThrowException_When_InValidData()
    {
        // Arrange
        var slotsHelper = new SlotsHelper(new DateHelper());
        var scheduleData = JsonConvert.DeserializeObject<ScheduleData>(inValidScheduleJson);

        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(() => slotsHelper.GetBusySlots(scheduleData));
    }

    [Fact]
    public void FormatCalendarEventsForFullCalendar_ValidInput_ShouldFormatCorrectly()
    {
        // Arrange
        var slotsHelper = new SlotsHelper(new DateHelper()); 
        var availableSlots = new List<string>
            {
                "08/07/2024 16:30:00 - 08/07/2024 16:40:00",
                "08/07/2024 16:40:00 - 08/07/2024 16:50:00"
            };
        var expectedEvents = new List<object>
            {
                new { start = "2024-07-08T16:30:00", end = "2024-07-08T16:40:00" },
                new { start = "2024-07-08T16:40:00", end = "2024-07-08T16:50:00" }
            };

        // Act
        var result = slotsHelper.FormatCalendarEventsForFullCalendar(availableSlots);

        // Assert
        Assert.Equal(expectedEvents.ToString(), result.ToString());
    }

    [Fact]
    public void FormatCalendarEventsForFullCalendar_Should_ThrowFormatException_WhenInvalidInput()
    {
        // Arrange
        var slotsHelper = new SlotsHelper(null); 
        var availableSlots = new List<string>
            {
                "08/07/2024 16:30:00 - InvalidEndDate"
            };

        // Act & Assert
        Assert.Throws<FormatException>(() => slotsHelper.FormatCalendarEventsForFullCalendar(availableSlots));
    }


    public static string validScheduleJson = @"
            {
                ""Facility"": {
                    ""FacilityId"": ""7960f43f-67dc-4bc5-a52b-9a3494306749"",
                    ""Name"": ""Test Practice"",
                    ""Address"": ""Test Address""
                },
                ""SlotDurationMinutes"": 10,
                ""Monday"": {
                    ""WorkPeriod"": {
                        ""StartHour"": 9,
                        ""EndHour"": 17,
                        ""LunchStartHour"": 13,
                        ""LunchEndHour"": 14
                    },
                    ""BusySlots"": [
                        {
                            ""Start"": ""2024-07-08T09:00:00"",
                            ""End"": ""2024-07-08T09:10:00""
                        },
                        {
                            ""Start"": ""2024-07-08T09:10:00"",
                            ""End"": ""2024-07-08T09:20:00""
                        },
                        {
                            ""Start"": ""2024-07-08T10:00:00"",
                            ""End"": ""2024-07-08T10:10:00""
                        }
                    ]
                },
                ""Tuesday"": null,
                ""Wednesday"": {
                    ""WorkPeriod"": {
                        ""StartHour"": 9,
                        ""EndHour"": 17,
                        ""LunchStartHour"": 13,
                        ""LunchEndHour"": 14
                    },
                    ""BusySlots"": [
                        {
                            ""Start"": ""2024-07-10T14:00:00"",
                            ""End"": ""2024-07-10T14:10:00""
                        },
                        {
                            ""Start"": ""2024-07-10T16:50:00+01:00"",
                            ""End"": ""2024-07-10T17:00:00+01:00""
                        }
                    ]
                },
                ""Thursday"": null,
                ""Friday"": null,
                ""Saturday"": null,
                ""Sunday"": null
            }";

    public static string inValidScheduleJson = @"
            {
                ""Facility"": {
                    ""FacilityId"": ""7960f43f-67dc-4bc5-a52b-9a3494306749"",
                    ""Name"": ""Test Practice"",
                    ""Address"": ""Test Address""
                },
                ""SlotDurationMinutes"": 10,
                ""Monday"": {
                    ""WorkPeriod"": {
                        ""EndHour"": 17,
                        ""LunchStartHour"": 13,
                        ""LunchEndHour"": 14
                    },
                    ""BusySlots"": [
                        {
                            ""Start"": ""2024-07-08T09:00:00"",
                            ""End"": ""2024-07-08T09:10:00""
                        },
                        {
                            ""Start"": ""2024-07-08T09:10:00"",
                            ""End"": ""2024-07-08T09:20:00""
                        },
                        {
                            ""Start"": ""2024-07-08T10:00:00"",
                            ""End"": ""2024-07-08T10:10:00""
                        }
                    ]
                },
                ""Tuesday"": null,
                ""Wednesday"":null,
                ""Thursday"": null,
                ""Friday"": null,
                ""Saturday"": null,
                ""Sunday"": null
            }";
}