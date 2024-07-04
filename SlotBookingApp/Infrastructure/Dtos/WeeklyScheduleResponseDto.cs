using Newtonsoft.Json;

namespace SlotBookingApp.Infrastructure.Dtos;

public class Facility
{
    [JsonProperty("FacilityId")]
    public Guid FacilityId { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("Address")]
    public string Address { get; set; }
}

public class WorkPeriod
{
    [JsonProperty("StartHour")]
    public int StartHour { get; set; }

    [JsonProperty("EndHour")]
    public int EndHour { get; set; }

    [JsonProperty("LunchStartHour")]
    public int LunchStartHour { get; set; }

    [JsonProperty("LunchEndHour")]
    public int LunchEndHour { get; set; }
}

public class BusySlot
{
    [JsonProperty("Start")]
    public DateTime Start { get; set; }

    [JsonProperty("End")]
    public DateTime End { get; set; }
}

public class DaySchedule
{
    [JsonProperty("WorkPeriod")]
    public WorkPeriod WorkPeriod { get; set; }

    [JsonProperty("BusySlots")]
    public List<BusySlot> BusySlots { get; set; }
}

public class ScheduleData
{
    [JsonProperty("Facility")]
    public Facility Facility { get; set; }

    [JsonProperty("SlotDurationMinutes")]
    public int SlotDurationMinutes { get; set; }

    [JsonProperty("Monday")]
    public DaySchedule Monday { get; set; }

    [JsonProperty("Tuesday")]
    public DaySchedule Tuesday { get; set; }

    [JsonProperty("Wednesday")]
    public DaySchedule Wednesday { get; set; }

    [JsonProperty("Thursday")]
    public DaySchedule Thursday { get; set; }

    [JsonProperty("Friday")]
    public DaySchedule Friday { get; set; }

    [JsonProperty("Saturday")]
    public DaySchedule Saturday { get; set; }

    [JsonProperty("Sunday")]
    public DaySchedule Sunday { get; set; }
}
