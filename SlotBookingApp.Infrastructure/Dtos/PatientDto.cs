using Newtonsoft.Json;

namespace SlotBookingApp.Infrastructure.Dtos;

public class PatientDto
{
    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("SecondName")]
    public string SecondName { get; set; }

    [JsonProperty("Email")]
    public string Email { get; set; }

    [JsonProperty("Phone")]
    public string Phone { get; set; }
}
