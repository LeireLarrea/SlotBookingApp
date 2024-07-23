using SlotBookingApp.Infrastructure.Dtos;
using System.Collections.Concurrent;
using System.Globalization;


namespace SlotBookingApp.Helpers;

/// <summary>
/// Helper class for managing slots and calendar events.
/// </summary>
public class SlotsHelper
{
    private readonly DateHelper _dateHelper;

    public SlotsHelper(DateHelper dateHelper)
    {
        _dateHelper = dateHelper;
    }

    /// <summary>
    /// Retrieves all available slots for the specified schedule based on WorkPeriod data and SlotDurationMinutes
    /// </summary>
    /// <param name="scheduleData">The schedule data.</param>
    /// <param name="date">The date in "yyyy-MM-dd" format for which slots are requested.</param>
    /// <returns>A list of strings representing all available slots.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the scheduleData or date are null</exception>

    public async Task<List<string>> GetAllSlots(ScheduleData scheduleData, string date)
    {
        if (scheduleData == null)
        {
            throw new ArgumentNullException(nameof(scheduleData), "ScheduleData cannot be null.");
        }

        if (date == null)
        {
            throw new ArgumentNullException(nameof(date), "Date cannot be null.");
        }

        var monday = _dateHelper.GetWeeksMonday(date);
        var daySchedules = GetDaySchedules(scheduleData);
        var tasks = new List<Task>();
        var allSlots = new ConcurrentBag<string>();

        foreach (var dayOfWeek in Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>())
        {
            var currentDay = daySchedules[dayOfWeek];
            if (currentDay?.WorkPeriod != null)
            {
                var dayToAdd = monday.AddDays((int)dayOfWeek - (int)DayOfWeek.Monday);

                tasks.Add(Task.Run(() =>
                {
                    var slotsForDay = GetSlotsInPeriod(currentDay.WorkPeriod.StartHour, currentDay.WorkPeriod.LunchStartHour, scheduleData.SlotDurationMinutes, dayToAdd);
                    slotsForDay.AddRange(GetSlotsInPeriod(currentDay.WorkPeriod.LunchEndHour, currentDay.WorkPeriod.EndHour, scheduleData.SlotDurationMinutes, dayToAdd));

                    foreach (var slot in slotsForDay)
                    {
                        allSlots.Add(slot);
                    }
                }));
            }
        }

        await Task.WhenAll(tasks);

        return allSlots.ToList();
    }



    private List<string> GetSlotsInPeriod(int startHour, int endHour, int slotDuration, DateTime date)
    {
        if (slotDuration <= 0)
        {
            throw new ArgumentException("Slot duration must be positive.", nameof(slotDuration));
        }
        if (startHour >= endHour)
        {
            throw new ArgumentException("Start hour must be less than end hour.", nameof(startHour));
        }

        DateTime periodStart = date.Date.AddHours(startHour);
        DateTime periodEnd = date.Date.AddHours(endHour);

        return _dateHelper.GenerateTimeList(periodStart, periodEnd, slotDuration);
    }


    /// <summary>
    /// Retrieves busy slots from the schedule data.
    /// </summary>
    /// <param name="scheduleData">The schedule data.</param>
    /// <returns>A list of strings of the busy slots.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the scheduleData is null</exception>
    public async Task<List<string>> GetBusySlots(ScheduleData scheduleData)
    {
        if (scheduleData == null)
        {
            throw new ArgumentNullException(nameof(scheduleData), "ScheduleData cannot be null.");
        }

        var daySchedules = GetDaySchedules(scheduleData);

        var formattedSlots = new ConcurrentBag<string>();
        var tasks = daySchedules.Select(async kvp =>
        {
            var daySchedule = kvp.Value;
            if (daySchedule?.BusySlots != null)
            {
                foreach (var busySlot in daySchedule.BusySlots)
                {
                    string formattedSlot = $"{busySlot.Start:dd/MM/yyyy HH:mm:ss} - {busySlot.End:dd/MM/yyyy HH:mm:ss}";
                    formattedSlots.Add(formattedSlot);
                }
            }
        }).ToArray();

        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while processing the schedule data.", ex);
        }

        return formattedSlots.ToList();
    }

    /// <summary>
    /// Formats a list of available slots to a format that FullCalendar.io understands
    /// </summary>
    /// <param name="availableSlots">A list of strings with all the available slots.</param>
    /// <returns>A list of objects of the available slots.</returns>
    /// <exception cref="FormatException">Thrown when the datetime parsing fails</exception>
    public List<object> FormatCalendarEventsForFullCalendar(List<string> availableSlots)
    {
        try
        {
            var tasks = availableSlots.Select(async slot =>
            {
                string[] times = slot.Split(" - ");
                string startTimeString = times[0];
                string endTimeString = times[1];

                DateTime startDateTime = DateTime.ParseExact(startTimeString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime endDateTime = DateTime.ParseExact(endTimeString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                string startISO = startDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                string endISO = endDateTime.ToString("yyyy-MM-ddTHH:mm:ss");

                return new
                {
                    start = startISO,
                    end = endISO
                };
            });

            var results = Task.WhenAll(tasks).Result;
            return results.Cast<object>().ToList();
        }
        catch (Exception ex)
        {
            throw new FormatException($"Error parsing slot times: {ex.Message}");
        }
    }


    private Dictionary<DayOfWeek, DaySchedule> GetDaySchedules(ScheduleData scheduleData)
    {
        return Enum.GetValues(typeof(DayOfWeek))
            .Cast<DayOfWeek>()
            .ToDictionary(day => day, day => GetDaySchedule(day, scheduleData));
    }

    private DaySchedule GetDaySchedule(DayOfWeek day, ScheduleData scheduleData)
    {
        return day switch
        {
            DayOfWeek.Monday => scheduleData.Monday,
            DayOfWeek.Tuesday => scheduleData.Tuesday,
            DayOfWeek.Wednesday => scheduleData.Wednesday,
            DayOfWeek.Thursday => scheduleData.Thursday,
            DayOfWeek.Friday => scheduleData.Friday,
            DayOfWeek.Saturday => scheduleData.Saturday,
            DayOfWeek.Sunday => scheduleData.Sunday,
            _ => null
        };
    }

}