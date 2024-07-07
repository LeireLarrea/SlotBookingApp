using System.Text;


namespace SlotBookingApp.Helpers;

/// <summary>
/// Helper class for managing HttpClient instances with basic authentication.
/// </summary>
public class HttpHelper
{
    private readonly IConfiguration _configuration;

    public HttpHelper(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public string GetAuthHeaderValue()
    {
        var username = _configuration["DraliatestSettings:Username"];
        var password = _configuration["DraliatestSettings:Password"];
        return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
    }
}
