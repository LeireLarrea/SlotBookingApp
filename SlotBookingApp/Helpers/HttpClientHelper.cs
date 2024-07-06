using System.Net.Http.Headers;
using System.Text;

namespace SlotBookingApp.Helpers;

/// <summary>
/// Helper class for managing HttpClient instances with basic authentication.
/// </summary>
public class HttpClientHelper
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private HttpClient _client;

    public HttpClientHelper(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        InitializeClient();
    }


    private void InitializeClient()
    {
        _client = _httpClientFactory.CreateClient();
        _client.BaseAddress = new Uri(_configuration["DraliatestSettings:BaseUrl"]);

        var authHeaderValue = GetAuthHeaderValue();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authHeaderValue);
    }

    /// <summary>
    /// Initializes and retrieves a HttpClient instance with base URL and basic authentication header.
    /// </summary>
    /// <returns>The configured HttpClient instance.</returns>
    public HttpClient GetClient()
    {
        return _client;
    }

    private string GetAuthHeaderValue()
    {
        var username = _configuration["DraliatestSettings:Username"];
        var password = _configuration["DraliatestSettings:Password"];
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    }
}
