using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

using SlotBookingApp.Models;


namespace SlotBookingApp.Swagger;

public class SwaggerUtility : Controller
{
    private readonly HttpClient _httpClient;

    public SwaggerUtility(IHttpClientFactory clientFactory, IConfiguration configuration)
    {
        _httpClient = clientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri(configuration["LocalhostSettings:BaseUrl"]);
    }

    /// <summary>
    /// A utility controller to allow Swagger access the HomeController.GetAvailableSlots() GET method without breaking the HomeController's MVC binding.
    /// Retrieves available slots for the specified week based on a date of that week
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///     GET /api/swagger/available-slots/2024-07-15
    ///     {
    ///     }
    /// </remarks>
    /// <param name="date">The date for which slots are requested (format: yyyy-MM-dd).</param>
    /// <returns>A list of available slots.</returns>
    [HttpGet("api/swagger/available-slots")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [Produces("application/json")]
    public async Task<IActionResult> GetAvailableSlots(string date)
    {
        var requestUrl = $"/Home/GetAvailableSlots?date={date}";

        try
        {
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var slots = await response.Content.ReadAsStringAsync();
                return Ok(slots);
            }
            else
            {
                return BadRequest("Failed to retrieve available slots.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// A utility controller to allow Swagger access the HomeController.BookEvent() POST method without breaking the HomeController's MVC binding.
    /// Books an event/slot using the selected event data.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///     POST api/swagger/book-event
    ///     {
    ///         "start": "2024-07-12T08:40:00+01:00",
    ///         "end": "2024-07-12T08:50:00+01:00",
    ///         "name": "YourName",
    ///         "secondName": "YourSurname",
    ///         "email": "email@email.com",
    ///         "phone": "12345678901",
    ///         "comments": "I just like doctors",
    ///         "facilityId": "7960f43f-67dc-4bc5-a52b-9a3494306749"
    ///     }
    ///     
    /// </remarks>
    /// <param name="eventData">The event data for booking (required).</param>
    /// <returns>The result of the booking operation.</returns>
    [HttpPost("api/swagger/book-event")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    [Produces("application/json")]
    public async Task<IActionResult> BookEvent([FromBody] CalendarEventModel eventData)
    {
        if (eventData == null)
        {
            return BadRequest("Slot data is required.");
        }

        var requestUrl = "/Home/BookEvent";

        try
        {
            var jsonContent = JsonConvert.SerializeObject(eventData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return Ok(result);
            }
            else
            {
                var errorDetails = await response.Content.ReadAsStringAsync();
                return BadRequest($"Failed to book event: {errorDetails}");
            }
        }
        catch (HttpRequestException e)
        {
            return StatusCode(500, $"Internal server error: {e.Message}");
        }
    }

    public IActionResult Index()
    {
        return View();
    }
}
