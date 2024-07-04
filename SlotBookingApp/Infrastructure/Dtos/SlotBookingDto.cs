using Newtonsoft.Json;

namespace SlotBookingApp.Infrastructure.Dtos;

public class SlotBookingDto
{
    [JsonProperty("FacilityId")]
    public string FacilityId { get; set; }

    [JsonProperty("Start")]
    public string Start { get; set; }

    [JsonProperty("End")]
    public string End { get; set; }

    [JsonProperty("Comments")]
    public string Comments { get; set; }

    [JsonProperty("Patient")]
    public PatientDto Patient { get; set; }
}
