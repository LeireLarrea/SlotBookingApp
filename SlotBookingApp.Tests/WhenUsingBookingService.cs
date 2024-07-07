using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

using SlotBookingApp.Helpers;
using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Infrastructure.Services;
using SlotBookingApp.Models;


namespace SlotBookingApp.Tests;

public class WhenUsingBookingService
{
    private readonly ILogger<BookingService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _mockHttpClient;
    private readonly MockHttpMessageHandler _mockHandler;

    private readonly HttpHelper _httpHelper;
    private readonly BookingService _bookingService;

    public WhenUsingBookingService()
    {
        _logger = Substitute.For<ILogger<BookingService>>();

        _configuration = Substitute.For<IConfiguration>();
        _configuration["DraliatestSettings:BaseUrl"].Returns("https://example.com/");
        _configuration["DraliatestSettings:Username"].Returns("testuser");
        _configuration["DraliatestSettings:Password"].Returns("testpassword");

        _mockHandler = new MockHttpMessageHandler();
        _mockHttpClient = new HttpClient(_mockHandler)
        {
            BaseAddress = new Uri(_configuration["DraliatestSettings:BaseUrl"])
        };

        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _httpClientFactory.CreateClient().Returns(_mockHttpClient);

        _httpHelper = Substitute.For<HttpHelper>(_httpClientFactory, _configuration);
        _httpHelper.GetAuthHeaderValue().Returns("base64encodedusernamepassword");
        _bookingService = new BookingService(_logger, _configuration, _httpClientFactory, _httpHelper);
    }

    [Fact]
    public async Task CreateBookingFromCalendarEvent_Should_CreateSlotBookingDto_WhenValidInput()
    {
        // Arrange
        var calendarEvent = ValidCalendarEvent();

        // Act
        var result = await _bookingService.CreateBookingFromCalendarEvent(calendarEvent);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Start.Should().Be(calendarEvent.Start);
            result.End.Should().Be(calendarEvent.End);
            result.Comments.Should().Be(calendarEvent.Comments);
            result.FacilityId.Should().Be(calendarEvent.FacilityId);

            result.Patient.Should().NotBeNull();
            result.Patient.Name.Should().Be(calendarEvent.Name);
            result.Patient.SecondName.Should().Be(calendarEvent.SecondName);
            result.Patient.Email.Should().Be(calendarEvent.Email);
            result.Patient.Phone.Should().Be(calendarEvent.Phone);
        }
    }

    [Fact]
    public async Task SendSlotBooking_Should_ReturnConfirmationWithStatusCode()
    {
        // Arrange
        var calendarEvent = ValidCalendarEvent();
        _mockHandler.SendAsyncFunc = request => new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        var confirmation = await _bookingService.SendSlotBooking(calendarEvent);

        // Assert
        using (new AssertionScope())
        {
            confirmation.Should().NotBeNull();
            confirmation.Name.Should().Be(calendarEvent.Name);
            confirmation.Slot.Should().Be(calendarEvent.Start);
            confirmation.Status.Should().Be(((int)HttpStatusCode.OK).ToString());
        }
    }

    [Fact]
    public async Task SendSlotBooking_Should_CallPostSlotBookingAndReturnCorrectStatus()
    {
        // Arrange
        var calendarEvent = ValidCalendarEvent();
        _mockHandler.SendAsyncFunc = request => new HttpResponseMessage(HttpStatusCode.Created);

        // Act
        var confirmation = await _bookingService.SendSlotBooking(calendarEvent);

        // Assert
        using (new AssertionScope())
        {
            confirmation.Should().NotBeNull();
            confirmation.Name.Should().Be(calendarEvent.Name);
            confirmation.Slot.Should().Be(calendarEvent.Start);
            confirmation.Status.Should().Be(((int)HttpStatusCode.Created).ToString());
        }
    }

    [Fact]
    public async Task PostSlotBooking_Should_ReturnStatusCode200_OnSuccess()
    {
        // Arrange
        var slotBookingDto = ValidSlotBookingDto();
        _mockHandler.SendAsyncFunc = request => new HttpResponseMessage(HttpStatusCode.OK);

        // Act
        var statusCode = await _bookingService.SendSlotBooking(ValidCalendarEvent());

        // Assert
        statusCode.Status.Should().Be(((int)HttpStatusCode.OK).ToString());
    }

    [Fact]
    public async Task PostSlotBooking_Should_ReturnStatusCode500_OnFailure()
    {
        // Arrange
        var slotBookingDto = ValidSlotBookingDto();
        _mockHandler.SendAsyncFunc = request => throw new HttpRequestException("An error occurred");

        // Act
        var statusCode = await _bookingService.SendSlotBooking(ValidCalendarEvent());

        // Assert
        statusCode.Status.Should().Be(((int)HttpStatusCode.InternalServerError).ToString());
    }

    private static CalendarEventModel ValidCalendarEvent()
    {
        return new CalendarEventModel
        {
            Start = "2024-07-12T08:40:00+01:00",
            End = "2024-07-12T08:50:00+01:00",
            Name = "TestName",
            SecondName = "TesSecondName",
            Email = "email@test.com",
            Phone = "01234567890",
            Comments = "I just like doctors",
            FacilityId = Guid.NewGuid().ToString()
        };
    }

    private static SlotBookingDto ValidSlotBookingDto()
    {
        var calendarEvent = ValidCalendarEvent();
        return new SlotBookingDto
        {
            Start = calendarEvent.Start,
            End = calendarEvent.End,
            Comments = calendarEvent.Comments,
            FacilityId = calendarEvent.FacilityId,
            Patient = new PatientDto
            {
                Name = calendarEvent.Name,
                SecondName = calendarEvent.SecondName,
                Email = calendarEvent.Email,
                Phone = calendarEvent.Phone
            }
        };
    }
}
