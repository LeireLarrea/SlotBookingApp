using SlotBookingApp.Infrastructure.Dtos;

namespace SlotBookingApp.Infrastructure.Helpers;

public class SlotsHelper
{
    private readonly DateHelper _dateHelper;
    public SlotsHelper(DateHelper dateHelper)
    {
        _dateHelper = dateHelper;
    }

    public async Task<List<string>> GetAllSlots(ScheduleData scheduleData, string date)
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

    public List<string> GetSlotsInPeriod(int startTime, int endTime, int slotDuration, DateTime date)
    {
        DateTime startHourTime = date.Date.AddHours(startTime);
        DateTime lunchStartHourTime = date.Date.AddHours(endTime);

        List<string> periodSlots = _dateHelper.GenerateTimeList(startHourTime, lunchStartHourTime, slotDuration);

        return periodSlots;
    }

    internal async Task<List<string>> GetBusySlots(ScheduleData scheduleData)
    {
        var formattedSlots = new List<string>();

        Parallel.ForEach(new[] { scheduleData.Monday, scheduleData.Tuesday, scheduleData.Wednesday, scheduleData.Friday },
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
}