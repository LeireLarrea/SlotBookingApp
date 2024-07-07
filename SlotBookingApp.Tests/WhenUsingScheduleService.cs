using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using System.Net;
using System.Text;

using SlotBookingApp.Helpers;
using SlotBookingApp.Infrastructure.Dtos;
using SlotBookingApp.Infrastructure.Services;


namespace SlotBookingApp.Tests;

public class WhenUsingScheduleService
{
    private readonly ILogger<ScheduleService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _mockHttpClient;
    private readonly MockHttpMessageHandler _mockHandler;

    private readonly HttpHelper _httpHelper;
    private readonly DateHelper _dateHelper;
    private readonly SlotsHelper _slotsHelper;
    private readonly ScheduleService _scheduleService;

    public WhenUsingScheduleService()
    {
        _logger = Substitute.For<ILogger<ScheduleService>>();

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

        _dateHelper = Substitute.For<DateHelper>();
        _slotsHelper = Substitute.For<SlotsHelper>(_dateHelper);

        _scheduleService = new ScheduleService(_logger, _configuration, _httpClientFactory, _dateHelper, _slotsHelper, _httpHelper);
    }

    [Fact]
    public async Task GetSchedule_Should_Return_ScheduleData_On_Successful_Response()
    {
        // Arrange
        var date = "2024-07-12";
        var expectedScheduleData = JsonConvert.DeserializeObject<ScheduleData>(validScheduleJson);

        _mockHandler.SendAsyncFunc = request => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(validScheduleJson, Encoding.UTF8, "application/json"),
        };

        // Act
        var result = await _scheduleService.GetSchedule(date);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedScheduleData);
        }
    }


    [Fact]
    public async Task GetSchedule_Should_Log_Error_And_Return_Empty_ScheduleData_On_Failure()
    {
        // Arrange
        var date = "2024-07-12";
        _mockHandler.SendAsyncFunc = request => new HttpResponseMessage(HttpStatusCode.InternalServerError);

        // Act
        var result = await _scheduleService.GetSchedule(date);

        // Assert
        Assert.ThrowsAsync<FormatException>(() => _scheduleService.GetSchedule(date));
        result.Should().BeEquivalentTo(new ScheduleData() { });
    }

    public static string validScheduleJson = @"{
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
    }
}";


}
