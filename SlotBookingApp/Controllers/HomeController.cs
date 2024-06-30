using Microsoft.AspNetCore.Mvc;
using SlotBookingApp.Helpers;
using SlotBookingApp.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using SlotBookingApp.Helpers;

namespace SlotBookingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DateHelper _dateHelper;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, DateHelper dateHelper, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _dateHelper = dateHelper;

            _httpClient = httpClientFactory.CreateClient("ExternalApi");
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes("techuser:secretpassWord"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetEvents(string date)
        {
            var response = await _httpClient.GetAsync($"availability/GetWeeklyAvailability/{_dateHelper.GetWeeksMonday(date)}");
            if (response.IsSuccessStatusCode)
            {
                var events = await response.Content.ReadFromJsonAsync<List<CalendarEvent>>();
                return Json(events);
            }
            else
            {
                // Handle error or return an empty list
                return Json(new List<CalendarEvent>());
            }
        }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // todo move to its own class
        public class CalendarEvent
        {
            public string Title { get; set; }
            public string Start { get; set; }
            public string End { get; set; }
        }
    }
}
