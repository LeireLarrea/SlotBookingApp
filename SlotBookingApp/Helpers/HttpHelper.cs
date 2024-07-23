using System.Text;


namespace SlotBookingApp.Helpers;

/// <summary>
/// Helper class for managing HttpClient instances with basic authentication.
/// </summary>
/// <param name="configuration">DI for IConfiguration</param>
/// <returns>Base 64 encoded credentials</returns>
/// <exception cref="ArgumentNullException">Thrown when either the configuration, or required keys are missing</exception>
public class HttpHelper
{
    private readonly IConfiguration _configuration;
    private readonly string _username;
    private readonly string _password;

    private const string UsernameKey = "DraliatestSettings:Username";
    private const string PasswordKey = "DraliatestSettings:Password";

    public HttpHelper(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _username = _configuration[UsernameKey] ?? throw new ArgumentNullException($"Configuration key '{UsernameKey}' is missing or empty.");
        _password = _configuration[PasswordKey] ?? throw new ArgumentNullException($"Configuration key '{PasswordKey}' is missing or empty.");
    }

    public string GetAuthHeaderValue()
    {
        var credentials = $"{_username}:{_password}";
        var base64Credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
        return $"Basic {base64Credentials}";
    }
}