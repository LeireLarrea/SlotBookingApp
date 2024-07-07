using SlotBookingApp.Infrastructure.Dtos;
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
    public async Task<List<string>> GetAllSlots(ScheduleData scheduleData, string date)
    {
        try
        {
            var monday = _dateHelper.GetWeeksMonday(date);
            var allSlots = new List<string>();

            var daySchedules = new[] { scheduleData.Monday, scheduleData.Tuesday, scheduleData.Wednesday,
                                   scheduleData.Thursday, scheduleData.Friday, scheduleData.Saturday,
                                   scheduleData.Sunday };

            Parallel.For(0, 7, i =>
            {
                var currentDay = daySchedules[i];
                if (currentDay != null && currentDay.WorkPeriod != null)
                {
                    var dayToAdd = monday.AddDays(i);
                    var slotsForDay = new List<string>();
                    slotsForDay.AddRange(GetSlotsInPeriod(currentDay.WorkPeriod.StartHour, currentDay.WorkPeriod.LunchStartHour, scheduleData.SlotDurationMinutes, dayToAdd));
                    slotsForDay.AddRange(GetSlotsInPeriod(currentDay.WorkPeriod.LunchEndHour, currentDay.WorkPeriod.EndHour, scheduleData.SlotDurationMinutes, dayToAdd));

                    lock (allSlots)
                    {
                        allSlots.AddRange(slotsForDay);
                    }
                }
            });

            return allSlots;
        }
        catch (Exception ex)
        {
            throw new ArgumentNullException($"Input data cannot be null. {ex}");
        }
    }

    private List<string> GetSlotsInPeriod(int startTime, int endTime, int slotDuration, DateTime date)
    {
        DateTime startHourTime = date.Date.AddHours(startTime);
        DateTime lunchStartHourTime = date.Date.AddHours(endTime);

        List<string> periodSlots = _dateHelper.GenerateTimeList(startHourTime, lunchStartHourTime, slotDuration);

        return periodSlots;
    }

    /// <summary>
    /// Retrieves busy slots from the schedule data.
    /// </summary>
    /// <param name="scheduleData">The schedule data.</param>
    /// <returns>A list of strings of the busy slots.</returns>
    public async Task<List<string>> GetBusySlots(ScheduleData scheduleData)
    {
        try
        {
            var formattedSlots = new List<string>();

            Parallel.ForEach(new[] { scheduleData.Monday, scheduleData.Tuesday, scheduleData.Wednesday,
                                   scheduleData.Thursday, scheduleData.Friday, scheduleData.Saturday,
                                   scheduleData.Sunday },
                (daySchedule) =>
                {
                    if (daySchedule != null && daySchedule.BusySlots != null)
                    {
                        foreach (var busySlot in daySchedule.BusySlots)
                        {
                            string formattedSlot = $"{busySlot.Start:dd/MM/yyyy HH:mm:ss} - {busySlot.End:dd/MM/yyyy HH:mm:ss}";
                            lock (formattedSlots)
                            {
                                formattedSlots.Add(formattedSlot);
                            }
                        }
                    }
                });

            return formattedSlots;
        }
        catch (Exception ex)
        {
            throw new ArgumentNullException($"Input data cannot be null. {ex}");
        }
    }

    public List<object> FormatCalendarEventsForFullCalendar(List<string> availableSlots)
    {
        try
        {
            List<object> events = new List<object>();

            Parallel.ForEach(availableSlots, slot =>
            {
                string[] times = slot.Split(" - ");
                string startTimeString = times[0];
                string endTimeString = times[1];

                DateTime startDateTime = DateTime.ParseExact(startTimeString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime endDateTime = DateTime.ParseExact(endTimeString, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                string startISO = startDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
                string endISO = endDateTime.ToString("yyyy-MM-ddTHH:mm:ss");

                events.Add(new
                {
                    start = startISO,
                    end = endISO
                });
            });

            return events;
        }
        catch (Exception ex)
        {
            throw new FormatException($"Error parsing slot times: {ex.Message}");
        }
    }
}